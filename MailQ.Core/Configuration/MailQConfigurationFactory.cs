// ReSharper disable NotResolvedInText

using System.Diagnostics.CodeAnalysis;

namespace MailQ.Core.Configuration;

[ExcludeFromCodeCoverage]
public class MailQConfigurationFactory : IMailQConfigurationFactory
{
    public MailQConfiguration LoadFromEnvironmentVariables() =>
        new()
        {
            EmailHost = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? throw new ArgumentNullException("EMAIL_HOST"),
            EmailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? throw new ArgumentNullException("EMAIL_PORT")),
            EmailUser = Environment.GetEnvironmentVariable("EMAIL_USER") ?? throw new ArgumentNullException("EMAIL_USER"),
            EmailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new ArgumentNullException("EMAIL_PASSWORD"),
            EmailAlias = Environment.GetEnvironmentVariable("EMAIL_ALIAS") ?? throw new ArgumentNullException("EMAIL_ALIAS"),
            EmailAddress = Environment.GetEnvironmentVariable("EMAIL_ADDRESS") ?? throw new ArgumentNullException("EMAIL_ADDRESS"),
            RabbitMqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") ?? throw new ArgumentNullException("RABBITMQ_CONNECTION_STRING"),
            RabbitMqDataExchange = Environment.GetEnvironmentVariable("RABBITMQ_DATA_EXCHANGE") ?? throw new ArgumentNullException("RABBITMQ_DATA_EXCHANGE"),
            RabbitMqDataQueue = Environment.GetEnvironmentVariable("RABBITMQ_DATA_QUEUE") ?? throw new ArgumentNullException("RABBITMQ_DATA_QUEUE"),
            RabbitMqRoutingKey = Environment.GetEnvironmentVariable("RABBITMQ_ROUTING_KEY") ?? throw new ArgumentNullException("RABBITMQ_ROUTING_KEY")
        };
}