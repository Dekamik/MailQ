using Autofac;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Core.Polling;
using RabbitMQ.Client;

namespace MailQ.Core.DependencyInjection;

public class MailQModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterSingleton<IMailQConfigurationFactory, MailQConfigurationFactory>();
        
        builder.RegisterScoped<ISmtpClientFactory, SmtpClientFactory>();
        builder.RegisterSingleton<IEmailer, Emailer>();
        
        builder.RegisterScoped<IConnectionFactory, ConnectionFactory>();
        builder.RegisterScoped<IMimeConverter, MimeConverter>();
        builder.RegisterScoped<IConsumerFactory, ConsumerFactory>();
        builder.RegisterSingleton<IPoller, Poller>();
    }
}