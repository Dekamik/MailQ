using Autofac;
using MailQ.Core.Email;

namespace MailQ.Core.DependencyInjection;

public class MailQModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterScoped<ISmtpClientFactory, SmtpClientFactory>();
        builder.RegisterSingleton<IEmailer, Emailer>();
    }
}