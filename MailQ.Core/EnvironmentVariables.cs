namespace MailQ.Core;

public static class EnvironmentVariables
{
    public static readonly string? EmailHost = Environment.GetEnvironmentVariable("EMAIL_HOST");
    public static readonly int EmailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "25");
    public static readonly string? EmailUser = Environment.GetEnvironmentVariable("EMAIL_USER");
    public static readonly string? EmailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
}