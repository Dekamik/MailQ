using FakeItEasy;
using FluentAssertions;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Protobuf;
using MimeKit;

namespace MailQ.Core.Tests.Email;

public class MimeConverterTests
{
    private readonly MailQConfiguration _configuration;
    
    private readonly IMailQConfigurationFactory _configurationFactory;
    private readonly IMimeConverter _mimeConverter;
    
    public MimeConverterTests()
    {
        _configuration = new MailQConfiguration
        {
            EmailAddress = "any@anydomain.com",
            EmailAlias = "Anyone"
        };
        _configurationFactory = A.Fake<IMailQConfigurationFactory>();
        A.CallTo(() => _configurationFactory.LoadFromEnvironmentVariables())
            .Returns(_configuration);
        
        _mimeConverter = new MimeConverter(_configurationFactory);
    }

    [Fact]
    public void ToMimeMessage_AnyMail_ReturnsMailAsMimeMessage()
    {
        var mail = new Mail
        {
            Body = "Any",
            Subject = "AnySubject",
            To = { "any1@otherdomain.com", "any2@otherdomain.com" }
        };

        var mimeMessage = _mimeConverter.ToMimeMessage(mail);

        mimeMessage.Subject.Should().Be(mail.Subject);
        mimeMessage.To.Select(a => a.Name)
            .Should()
            .BeEquivalentTo(mail.To);
        mimeMessage.From.Single().Name
            .Should()
            .Be(_configuration.EmailAlias);
        ((TextPart)mimeMessage.Body).Text.Should().Be(mail.Body);
    }
}