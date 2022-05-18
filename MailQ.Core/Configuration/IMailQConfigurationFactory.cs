namespace MailQ.Core.Configuration;

public interface IMailQConfigurationFactory
{
    MailQConfiguration LoadFromEnvironmentVariables();
}