using FakeItEasy;
using Google.Protobuf;
using MailQ.Core.Configuration;
using MailQ.Core.Email;
using MailQ.Core.Polling;
using MailQ.Protobuf;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MailQ.Core.Tests.Polling;

public class PollerTests
{
    private readonly MailQConfiguration _configuration;
    private readonly IMimeConverter _mimeConverter;
    private readonly IEmailer _emailer;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IConsumerFactory _consumerFactory;

    private readonly Poller _poller;

    public PollerTests()
    {
        _configuration = new MailQConfiguration
        {
            RabbitMqDataExchange = "AnyExchange",
            RabbitMqDataQueue = "AnyQueue",
            RabbitMqRoutingKey = "AnyRoutingKey"
        };
        var configurationFactory = A.Fake<IMailQConfigurationFactory>();
        A.CallTo(() => configurationFactory.LoadFromEnvironmentVariables())
            .Returns(_configuration);

        _mimeConverter = A.Fake<IMimeConverter>();
        _emailer = A.Fake<IEmailer>();
        
        _channel = A.Fake<IModel>();
        _connection = A.Fake<IConnection>();
        A.CallTo(() => _connection.CreateModel())
            .Returns(_channel);

        _connectionFactory = A.Fake<IConnectionFactory>();
        A.CallTo(() => _connectionFactory.CreateConnection())
            .Returns(_connection);

        _consumerFactory = A.Fake<IConsumerFactory>();

        _poller = new Poller(configurationFactory, _connectionFactory, _emailer, _mimeConverter, _consumerFactory);
    }

    [Fact]
    public void Constructor_Any_CreatesConnectionAndModel()
    {
        A.CallTo(() => _connectionFactory.CreateConnection())
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _connection.CreateModel())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Dispose_Any_DisposesConnectionAndModel()
    {
        _poller.Dispose();

        A.CallTo(() => _channel.Dispose())
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _connection.Dispose())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void HandleMailEvent_Any_ShouldParseAndSendMail()
    {
        var mail = new Mail
        {
            Body = "Any",
            Subject = "AnySubject",
            To = { "any1@mail.com", "any2@gmail.com" }
        };
        var ea = new BasicDeliverEventArgs
        {
            Body = mail.ToByteArray(),
            DeliveryTag = 1L
        };
        var mimeMessage = new MimeMessage();

        A.CallTo(() => _mimeConverter.ToMimeMessage(mail))
            .Returns(mimeMessage);

        _poller.HandleMailEvent.Invoke(null, ea);

        A.CallTo(() => _mimeConverter.ToMimeMessage(mail))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _emailer.SendEmail(mimeMessage))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _channel.BasicAck(ea.DeliveryTag, false))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DoPolling_Any_InitializesPolling()
    {
        var cancellationToken = new CancellationTokenSource();
        cancellationToken.CancelAfter(250);

        await _poller.DoPolling(cancellationToken.Token);

        A.CallTo(() => _channel.ExchangeDeclare(_configuration.RabbitMqDataExchange, "direct", 
                true, false, default))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _channel.QueueDeclare(_configuration.RabbitMqDataQueue, false, false, 
                false, default))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _channel.QueueBind(_configuration.RabbitMqDataQueue, 
                _configuration.RabbitMqDataExchange, _configuration.RabbitMqRoutingKey, default))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _channel.BasicQos(0, 1, false))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerFactory.CreateEventingBasicConsumer(_channel))
            .MustHaveHappenedOnceExactly();
    }
}