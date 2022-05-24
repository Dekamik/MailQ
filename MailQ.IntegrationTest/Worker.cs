using Google.Protobuf;
using MailQ.Protobuf;
using RabbitMQ.Client;

namespace MailQ.IntegrationTest;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subject = Environment.GetEnvironmentVariable("EMAIL_SUBJECT")!;
        var body = Environment.GetEnvironmentVariable("EMAIL_BODY")!;
        var to = Environment.GetEnvironmentVariable("EMAIL_TO")!.Split(",");
        
        var rabbitmqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING")!;
        var exchange = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE");
        var queue = Environment.GetEnvironmentVariable("RABBITMQ_QUEUE");
        var routingKey = Environment.GetEnvironmentVariable("RABBITMQ_ROUTING_KEY");

        var connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(rabbitmqConnectionString)
        };

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue, false, false, false, null);
        
        _logger.LogInformation("Connection established");

        var mail = new Mail
        {
            Subject = subject,
            Body = body,
            To = { to }
        };
        var buffer = mail.ToByteArray();

        var properties = channel.CreateBasicProperties();
        properties.CorrelationId = Guid.NewGuid().ToString();
        channel.BasicPublish(exchange, routingKey, properties, buffer);
        
        _logger.LogInformation("Mail published!");
    }
}