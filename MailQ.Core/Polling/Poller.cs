using System.Diagnostics;
using MailQ.Core.Configuration;
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

    public Poller(IMailQConfigurationFactory configurationFactory, IConnectionFactory connectionFactory)
    {
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

                try
                {

                }
                finally
                {
                    stopwatch.Stop();
                    Log.Verbose("Processed {Message} in {Elapsed}", ea.Body.ToString(), stopwatch.Elapsed);
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