using MailQ.Core.Configuration;
using MailQ.Protobuf;
using MimeKit;

namespace MailQ.Core.Email;

public class MimeConverter : IMimeConverter
{
    private readonly MailQConfiguration _configuration;
    
    public MimeConverter(IMailQConfigurationFactory configurationFactory)
    {
        _configuration = configurationFactory.LoadFromEnvironmentVariables();
    }
    
    public MimeMessage ToMimeMessage(Mail mail)
    {
        var mimeMessage = new MimeMessage();
        
        mimeMessage.From.Add(new MailboxAddress(_configuration.EmailAlias, _configuration.EmailAddress));
        mimeMessage.To.AddRange(mail.To.Select(address => new MailboxAddress(address, address)));
        mimeMessage.Subject = mail.Subject;
        mimeMessage.Body = new TextPart("plain")
        {
            Text = mail.Body
        };
        
        return mimeMessage;
    }
}