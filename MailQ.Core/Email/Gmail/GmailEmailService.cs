using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Util.Store;
using MailQ.Core.Configuration;
using MimeKit;
using Serilog;

namespace MailQ.Core.Email.Gmail;

public class GmailEmailService : IGmailEmailService
{
    private readonly IGmailServiceFactory _gmailServiceFactory;

    private static string[] _scopes =
    {
        GmailService.Scope.GmailSend
    };
    private static string _applicationName = "MailQ";

    private readonly MailQConfiguration _configuration;

    public GmailEmailService(IMailQConfigurationFactory configurationFactory, IGmailServiceFactory gmailServiceFactory)
    {
        _gmailServiceFactory = gmailServiceFactory;
        _configuration = configurationFactory.LoadFromEnvironmentVariables();
    }
    
    public async Task SendEmail(MimeMessage message)
    {
        var credPath = "token.json";
        var clientSecrets = new ClientSecrets
        {
            ClientId = _configuration.GmailClientId,
            ClientSecret = _configuration.GmailClientSecret
        };
        
        Log.Information("Authorizing with {User}", _configuration.EmailUser);
        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets,
            _scopes,
            _configuration.EmailUser,
            CancellationToken.None,
            new FileDataStore(credPath, true));
        Log.Information("Authorized, credential file saved to {Path}", credPath);

        var service = _gmailServiceFactory.CreateGmailService(_applicationName, credential);
        
        message.From.Add(new MailboxAddress(_configuration.EmailAlias, _configuration.EmailAddress));
        var gmailMessage = new Message
        {
            Raw = Base64UrlEncode(message.ToString())
        };
        
        await service.Users.Messages.Send(gmailMessage, _configuration.EmailAddress)
            .ExecuteAsync();
        Log.Information("Email sent with Subject={Subject} and Body={Body} to {To}", 
            message.Subject, message.Body, string.Join(", ", message.To));
    }

    private static string Base64UrlEncode(string input)
    {
        var buffer = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(buffer)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}