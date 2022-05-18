using System.Diagnostics;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Protobuf;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;

namespace MailQ.Core.Polling;

public class Poller : IPoller, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly MailQConfiguration _configuration;
    private readonly IEmailer _emailer;

    public Poller(IMailQConfigurationFactory configurationFactory, IConnectionFactory connectionFactory, 
        IEmailer emailer)
    {
        _emailer = emailer;
        _configuration = configurationFactory.LoadFromEnvironmentVariables();
        _connection = connectionFactory.CreateConnection();
        Log.Information("Connection to RabbitMQ established");
        
        _channel = _connection.CreateModel();
        Log.Information("Channel created");
    }

    public async Task DoPolling(CancellationToken cancellationToken)
    {
        _channel.ExchangeDeclare(_configuration.RabbitMqDataExchange, "direct", true, false);
        _channel.QueueDeclare(_configuration.RabbitMqDataQueue, false, false, false);
        _channel.QueueBind(_configuration.RabbitMqDataQueue, _configuration.RabbitMqDataExchange,
            _configuration.RabbitMqRoutingKey);
        _channel.BasicQos(0, 1, false);

        var channel1 = _channel;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            using (LogContext.PushProperty("CorrelationId", ea.BasicProperties.CorrelationId))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    var mail = Mail.Parser.ParseFrom(ea.Body.ToArray());
                    Log.Information("Received email message with Subject={Subject} and Body={Body} to {To}", 
                        mail.Subject, mail.Body, string.Join(", ", mail.To));

                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress(_configuration.EmailAlias, _configuration.EmailAddress));
                    mimeMessage.To.AddRange(mail.To.Select(address => new MailboxAddress(address, address)));
                    mimeMessage.Subject = mail.Subject;
                    mimeMessage.Body = new TextPart("plain")
                    {
                        Text = mail.Body
                    };
                    
                    await _emailer.SendEmail(mimeMessage);
                }
                finally
                {
                    stopwatch.Stop();
                    Log.Verbose("Processed email in {Elapsed}", stopwatch.Elapsed);
                    channel1.BasicAck(ea.DeliveryTag, false);
                }
            }
        };
        
        Log.Information("Starting consumer");

        _channel.BasicConsume(_configuration.RabbitMqDataQueue, false, consumer);
        
        Log.Information("Consumer available");

        while (!cancellationToken.IsCancellationRequested)
        {
            
        }
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}