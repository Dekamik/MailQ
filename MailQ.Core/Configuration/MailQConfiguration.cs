namespace MailQ.Core.Configuration;

public class MailQConfiguration
{
    public string EmailHost { get; set; }
    public int EmailPort { get; set; }
    public string EmailUser { get; set; }
    public string EmailPassword { get; set; }
    
    public string RabbitMqConnectionString { get; set; }
    public string RabbitMqDataExchange { get; set; }
    public string RabbitMqDataQueue { get; set; }
    public string RabbitMqRoutingKey { get; set; }
}