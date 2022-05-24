using MailQ.Protobuf;
using MimeKit;

namespace MailQ.Core.Email;

public interface IEmailService
{
    Task SendEmail(MimeMessage message);
}