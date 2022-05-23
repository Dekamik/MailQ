using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MailQ.Core.Polling;

[ExcludeFromCodeCoverage]
public class ConsumerFactory : IConsumerFactory
{
    public EventingBasicConsumer CreateEventingBasicConsumer(IModel? channel) => new(channel);
}