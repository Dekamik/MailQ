using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Protobuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;

namespace MailQ.Core.Polling;

public class Poller : IPoller, IDisposable
{
    private readonly IConnection _connection;
    private static IModel _channel = null!;
    private readonly MailQConfiguration _configuration;
    private static IEmailer _emailer = null!;
    private static IMimeConverter _mimeConverter = null!;
    private readonly IConsumerFactory _consumerFactory;
    
    public readonly EventHandler<BasicDeliverEventArgs> HandleMailEvent = MailEvent;
    
    private static async void MailEvent(object? model, BasicDeliverEventArgs ea)
    {
        using (LogContext.PushProperty("CorrelationId", ea.BasicProperties?.CorrelationId ?? null))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var mail = Mail.Parser.ParseFrom(ea.Body.ToArray());
                Log.Information("Received email message with Subject={Subject} and Body={Body} to {To}", 
                    mail.Subject, mail.Body, string.Join(", ", mail.To));

                var mimeMessage = _mimeConverter.ToMimeMessage(mail);
                await _emailer.SendEmail(mimeMessage);
            }
            finally
            {
                stopwatch.Stop();
                Log.Information("Processed email in {Elapsed}", stopwatch.Elapsed);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }

    public Poller(IMailQConfigurationFactory configurationFactory, IConnectionFactory connectionFactory, 
        IEmailer emailer, IMimeConverter mimeConverter, IConsumerFactory consumerFactory)
    {
        _consumerFactory = consumerFactory;
        _mimeConverter = mimeConverter;
        _emailer = emailer;
        _configuration = configurationFactory.LoadFromEnvironmentVariables();
        _connection = connectionFactory.CreateConnection();
        Log.Information("Connection to RabbitMQ established");
        
        _channel = _connection.CreateModel();
        Log.Information("Channel created");
    }

    public Task DoPolling(CancellationToken cancellationToken)
    {
        _channel.ExchangeDeclare(_configuration.RabbitMqDataExchange, "direct", true, false);
        _channel.QueueDeclare(_configuration.RabbitMqDataQueue, false, false, false);
        _channel.QueueBind(_configuration.RabbitMqDataQueue, _configuration.RabbitMqDataExchange,
            _configuration.RabbitMqRoutingKey);
        _channel.BasicQos(0, 1, false);

        var consumer = _consumerFactory.CreateEventingBasicConsumer(_channel);
        consumer.Received += HandleMailEvent;
        
        Log.Information("Starting consumer");

        _channel.BasicConsume(_configuration.RabbitMqDataQueue, false, consumer);
        
        Log.Information("Consumer available");

        while (!cancellationToken.IsCancellationRequested)
        {
            
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}