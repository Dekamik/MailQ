using FakeItEasy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Core.Email.Generic;
using MimeKit;

namespace MailQ.Core.Tests.Email.Generic;

public class GenericEmailServiceTests
{
    private readonly ISmtpClient _smtp;
    private readonly GenericEmailService _genericEmailService;

    public GenericEmailServiceTests()
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
        
        _genericEmailService = new GenericEmailService(configurationFactory, smtpClientFactory);
    }

    [Fact]
    public void Constructor_Any_CreatesSmtpClientThroughFactory()
    {
        var configurationFactory = A.Fake<IMailQConfigurationFactory>();
        var clientFactory = A.Fake<ISmtpClientFactory>();

        var _ = new GenericEmailService(configurationFactory, clientFactory);

        A.CallTo(() => clientFactory.CreateSmtpClient())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Dispose_Any_DisposesSmtpClient()
    {
        _genericEmailService.Dispose();

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

        await _genericEmailService.SendEmail(message);

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

        await _genericEmailService.SendEmail(message);

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

        await _genericEmailService.SendEmail(message);

        A.CallTo(() => _smtp.AuthenticateAsync("AnyUser", "AnyPassword", default))
            .MustHaveHappenedOnceExactly();
    }
}