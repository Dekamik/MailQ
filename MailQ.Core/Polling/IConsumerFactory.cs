using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MailQ.Core.Polling;

public interface IConsumerFactory
{
    EventingBasicConsumer CreateEventingBasicConsumer(IModel? channel);
}