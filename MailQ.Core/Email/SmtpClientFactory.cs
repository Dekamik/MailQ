using System.Diagnostics.CodeAnalysis;
using MailKit.Net.Smtp;

namespace MailQ.Core.Email;

[ExcludeFromCodeCoverage]
public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient CreateSmtpClient() => new SmtpClient();
}