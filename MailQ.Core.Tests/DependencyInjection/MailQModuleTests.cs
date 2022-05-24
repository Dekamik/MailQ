using Autofac;
using Autofac.Core;
using FakeItEasy;
using FluentAssertions;
using MailQ.Core.Configuration;
using MailQ.Core.DependencyInjection;
using RabbitMQ.Client;
using Xunit.Abstractions;

namespace MailQ.Core.Tests.DependencyInjection;

public class MailQModuleTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MailQModuleTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Load_Any_ResolvesAllRegisteredDependencies()
    {
        Environment.SetEnvironmentVariable("RABBITMQ_CONNECTION_STRING", "amqp://localhost:0");
        
        var configuration = new MailQConfiguration();
        var configurationFactory = A.Fake<IMailQConfigurationFactory>();
        A.CallTo(() => configurationFactory.LoadFromEnvironmentVariables())
            .Returns(configuration);

        var connectionFactory = A.Fake<IConnectionFactory>();
        var connection = A.Fake<IConnection>();
        var channel = A.Fake<IModel>();
        A.CallTo(() => connectionFactory.CreateConnection())
            .Returns(connection);
        A.CallTo(() => connection.CreateModel())
            .Returns(channel);
        
        var builder = new ContainerBuilder();
        builder.RegisterModule<MailQModule>();
        builder.RegisterInstance(configurationFactory)
            .As<IMailQConfigurationFactory>();
        builder.RegisterInstance(connectionFactory)
            .As<IConnectionFactory>();

        var container = builder.Build();
        using var scope = container.BeginLifetimeScope();

        var distinctTypes = scope.ComponentRegistry.Registrations
            .SelectMany(r => r.Services.OfType<TypedService>().Select(s => s.ServiceType))
            .Distinct();
        
        var exceptions = new List<Exception>();
        var maxLength = distinctTypes.Max(t => t.FullName.Length);

        _testOutputHelper.WriteLine("{0, -" + maxLength + "}\t\tResult", "Service");
        foreach (var distinctType in distinctTypes)
        {
            try
            {
                container.Resolve(distinctType);
                _testOutputHelper.WriteLine("{0, -" + maxLength + "}\t\tOK", distinctType.FullName);
            }
            catch (DependencyResolutionException ex)
            {
                _testOutputHelper.WriteLine("{0, -" + maxLength + "}\t\tThrew {1}", distinctType.FullName, 
                    nameof(DependencyResolutionException));
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
            _testOutputHelper.WriteLine($"Check the variable \"{nameof(exceptions)}\" in debug mode for details");
        exceptions.Count.Should().Be(0);
    }
}