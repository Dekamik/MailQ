namespace MailQ.Worker;

public class EnvironmentVariables
{
    public static readonly string? RabbitMqConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING");
}