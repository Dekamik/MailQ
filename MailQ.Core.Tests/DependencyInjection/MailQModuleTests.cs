using Autofac;
using Autofac.Core;
using FakeItEasy;
using MailQ.Core.Configuration;
using MailQ.Core.DependencyInjection;

namespace MailQ.Core.Tests.DependencyInjection;

public class MailQModuleTests
{
    [Fact]
    public void Load_Any_ResolvesAllRegisteredDependencies()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<MailQModule>();

        var container = builder.Build();
        using var scope = container.BeginLifetimeScope(builder =>
        {
            var configuration = new MailQConfiguration();
            var configurationFactory = A.Fake<IMailQConfigurationFactory>();
            A.CallTo(() => configurationFactory.LoadFromEnvironmentVariables())
                .Returns(configuration);

            builder.RegisterInstance(configurationFactory)
                .As<IMailQConfigurationFactory>();
        });

        var distinctTypes = scope.ComponentRegistry.Registrations
            .SelectMany(r => r.Services.OfType<TypedService>().Select(s => s.ServiceType))
            .Distinct();

        foreach (var distinctType in distinctTypes)
        {
            container.Resolve(distinctType);
        }
    }
}