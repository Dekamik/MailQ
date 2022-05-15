using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MailQ.Core.Email;

public class Emailer : IEmailer, IDisposable 
{
    private readonly ISmtpClient _smtp;

    public Emailer(ISmtpClientFactory smtpClientFactory)
    {
        _smtp = smtpClientFactory.CreateSmtpClient();
    }

    public async Task SendEmail(MimeMessage message)
    {
        if (!_smtp.IsConnected)
            await _smtp.ConnectAsync(EnvironmentVariables.EmailHost, EnvironmentVariables.EmailPort, SecureSocketOptions.StartTls);

        if (!_smtp.IsAuthenticated)
            await _smtp.AuthenticateAsync(EnvironmentVariables.EmailUser, EnvironmentVariables.EmailPassword);

        await _smtp.SendAsync(message);
    }

    public void Dispose()
    {
        _smtp.Disconnect(true);
        _smtp.Dispose();
    }
}