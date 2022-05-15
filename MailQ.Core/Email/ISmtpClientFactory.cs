using MailKit.Net.Smtp;

namespace MailQ.Core.Email;

public interface ISmtpClientFactory
{
    ISmtpClient CreateSmtpClient();
}