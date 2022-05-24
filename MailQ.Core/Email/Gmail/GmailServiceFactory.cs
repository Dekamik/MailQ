using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;

namespace MailQ.Core.Email.Gmail;

public class GmailServiceFactory : IGmailServiceFactory
{
    public GmailService CreateGmailService(string applicationName, UserCredential credential) => new GmailService(new BaseClientService.Initializer
    {
        ApplicationName = applicationName,
        HttpClientInitializer = credential
    });
}