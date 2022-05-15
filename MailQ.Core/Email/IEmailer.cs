using MimeKit;

namespace MailQ.Core.Email;

public interface IEmailer
{
    Task SendEmail(MimeMessage message);
}