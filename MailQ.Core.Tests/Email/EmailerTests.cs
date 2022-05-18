using FakeItEasy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MimeKit;

namespace MailQ.Core.Tests.Email;

public class EmailerTests
{
    private readonly ISmtpClient _smtp;
    private readonly Emailer _emailer;

    public EmailerTests()
    {
        var configuration = new MailQConfiguration
        {
            EmailHost = "AnyHost",
            EmailPort = 25,
            EmailUser = "AnyUser",
            EmailPassword = "AnyPassword"
        };
        var configurationFactory = A.Fake<IMailQConfigurationFactory>();
        A.CallTo(() => configurationFactory.LoadFromEnvironmentVariables())
            .Returns(configuration);

        var smtpClientFactory = A.Fake<ISmtpClientFactory>();
        _smtp = A.Fake<ISmtpClient>();
        A.CallTo(() => smtpClientFactory.CreateSmtpClient())
            .Returns(_smtp);
        
        _emailer = new Emailer(configurationFactory, smtpClientFactory);
    }

    [Fact]
    public void Constructor_Any_CreatesSmtpClientThroughFactory()
    {
        var configurationFactory = A.Fake<IMailQConfigurationFactory>();
        var clientFactory = A.Fake<ISmtpClientFactory>();

        var _ = new Emailer(configurationFactory, clientFactory);

        A.CallTo(() => clientFactory.CreateSmtpClient())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Dispose_Any_DisposesSmtpClient()
    {
        _emailer.Dispose();

        A.CallTo(() => _smtp.Disconnect(true, default))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _smtp.Dispose())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendMail_ConnectedAndAuthenticated_CallsSendAsync()
    {
        var message = new MimeMessage();
        A.CallTo(() => _smtp.IsConnected)
            .Returns(true);
        A.CallTo(() => _smtp.IsAuthenticated)
            .Returns(true);

        await _emailer.SendEmail(message);

        A.CallTo(() => _smtp.SendAsync(message, default, null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendMail_Disconnected_Reconnects()
    {
        var message = new MimeMessage();
        A.CallTo(() => _smtp.IsConnected)
            .Returns(false);
        A.CallTo(() => _smtp.IsAuthenticated)
            .Returns(true);

        await _emailer.SendEmail(message);

        A.CallTo(() => _smtp.ConnectAsync("AnyHost", 25, SecureSocketOptions.StartTls, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendMail_NotAuthenticated_Authenticates()
    {
        var message = new MimeMessage();
        A.CallTo(() => _smtp.IsConnected)
            .Returns(true);
        A.CallTo(() => _smtp.IsAuthenticated)
            .Returns(false);

        await _emailer.SendEmail(message);

        A.CallTo(() => _smtp.AuthenticateAsync("AnyUser", "AnyPassword", default))
            .MustHaveHappenedOnceExactly();
    }
}