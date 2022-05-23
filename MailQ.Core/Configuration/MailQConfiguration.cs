using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618
namespace MailQ.Core.Configuration;

[ExcludeFromCodeCoverage]
public class MailQConfiguration
{
    public string EmailHost { get; set; }
    public int EmailPort { get; set; }
    public string EmailUser { get; set; }
    public string EmailPassword { get; set; }
    public string EmailAlias { get; set; }
    public string EmailAddress { get; set; }
    
    public string RabbitMqConnectionString { get; set; }
    public string RabbitMqDataExchange { get; set; }
    public string RabbitMqDataQueue { get; set; }
    public string RabbitMqRoutingKey { get; set; }
}