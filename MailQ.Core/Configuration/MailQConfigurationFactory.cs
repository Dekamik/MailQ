// ReSharper disable NotResolvedInText

using System.Diagnostics.CodeAnalysis;

namespace MailQ.Core.Configuration;

[ExcludeFromCodeCoverage]
public class MailQConfigurationFactory : IMailQConfigurationFactory
{
    public MailQConfiguration LoadFromEnvironmentVariables()
    {
        var config = new MailQConfiguration
        {
            EmailUser = Environment.GetEnvironmentVariable("EMAIL_USER") ??
                        throw new ArgumentNullException("EMAIL_USER"),
            EmailAlias = Environment.GetEnvironmentVariable("EMAIL_ALIAS") ??
                         throw new ArgumentNullException("EMAIL_ALIAS"),
            EmailAddress = Environment.GetEnvironmentVariable("EMAIL_ADDRESS") ??
                           throw new ArgumentNullException("EMAIL_ADDRESS"),
            
            RabbitMqDataExchange = Environment.GetEnvironmentVariable("RABBITMQ_DATA_EXCHANGE") ??
                                   throw new ArgumentNullException("RABBITMQ_DATA_EXCHANGE"),
            RabbitMqDataQueue = Environment.GetEnvironmentVariable("RABBITMQ_DATA_QUEUE") ??
                                throw new ArgumentNullException("RABBITMQ_DATA_QUEUE"),
            RabbitMqRoutingKey = Environment.GetEnvironmentVariable("RABBITMQ_ROUTING_KEY") ??
                                 throw new ArgumentNullException("RABBITMQ_ROUTING_KEY")
        };

        switch (EnvironmentVariables.EmailClient?.ToLower() ?? "generic")
        {
            case "gmail":
                config.GmailClientId = Environment.GetEnvironmentVariable("GMAIL_CLIENT_ID") ??
                                       throw new ArgumentNullException("GMAIL_CLIENT_ID");
                config.GmailClientSecret = Environment.GetEnvironmentVariable("GMAIL_CLIENT_SECRET") ??
                                           throw new ArgumentNullException("GMAIL_CLIENT_SECRET");
                break;
            
            case "generic":
            default:
                config.EmailHost = Environment.GetEnvironmentVariable("EMAIL_HOST") ??
                                   throw new ArgumentNullException("EMAIL_HOST");
                config.EmailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ??
                                             throw new ArgumentNullException("EMAIL_PORT"));
                config.EmailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ??
                                       throw new ArgumentNullException("EMAIL_PASSWORD");
                break;
        }

        return config;
    }
}