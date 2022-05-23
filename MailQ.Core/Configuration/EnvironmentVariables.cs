using System.Diagnostics.CodeAnalysis;

namespace MailQ.Core.Configuration;

[ExcludeFromCodeCoverage]
public static class EnvironmentVariables
{
    public static readonly string? RabbitMqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING");
    public static readonly string? LokiConnectionString = Environment.GetEnvironmentVariable("LOKI_CONNECTION_STRING");
}