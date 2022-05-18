using MailQ.Protobuf;
using MimeKit;

namespace MailQ.Core.Email;

public interface IMimeConverter
{
    MimeMessage ToMimeMessage(Mail mail);
}