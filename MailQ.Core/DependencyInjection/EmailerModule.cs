using Autofac;
using MailQ.Core.Email;

namespace MailQ.Core.DependencyInjection;

public class EmailerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterScoped<ISmtpClientFactory, SmtpClientFactory>();
        builder.RegisterSingleton<IEmailer, Emailer>();
    }
}