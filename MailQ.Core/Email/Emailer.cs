using MailKit.Net.Smtp;
using MailKit.Security;
using MailQ.Core.Configuration;
using MimeKit;
using Serilog;

namespace MailQ.Core.Email;

public class Emailer : IEmailer, IDisposable 
{
    private readonly ISmtpClient _smtp;
    private readonly MailQConfiguration _configuration;

    public Emailer(IMailQConfigurationFactory configurationFactory, ISmtpClientFactory smtpClientFactory)
    {
        _configuration = configurationFactory.LoadFromEnvironmentVariables();
        _smtp = smtpClientFactory.CreateSmtpClient();
    }

    public async Task SendEmail(MimeMessage message)
    {
        if (!_smtp.IsConnected)
        {
            await _smtp.ConnectAsync(_configuration.EmailHost, _configuration.EmailPort, 
                SecureSocketOptions.StartTls);
            Log.Information("Connected to email server at {Host}:{Port}", 
                _configuration.EmailHost, _configuration.EmailPort);
        }

        if (!_smtp.IsAuthenticated)
        {
            await _smtp.AuthenticateAsync(_configuration.EmailUser, _configuration.EmailPassword);
            Log.Information("Logged in as {User}", _configuration.EmailUser);
        }

        await _smtp.SendAsync(message);
        Log.Information("Email sent with Subject={Subject} and Body={Body} to {To}", 
            message.Subject, message.Body, string.Join(", ", message.To));
    }

    public void Dispose()
    {
        _smtp.Disconnect(true);
        Log.Information("Disconnected from email server");
        _smtp.Dispose();
    }
}