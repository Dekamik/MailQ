using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;

namespace MailQ.Core.Email.Gmail;

public interface IGmailServiceFactory
{
    GmailService CreateGmailService(string applicationName, UserCredential credential);
}