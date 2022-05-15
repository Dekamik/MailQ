using Autofac;
using Autofac.Core;
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
        using var scope = container.BeginLifetimeScope();

        var distinctTypes = scope.ComponentRegistry.Registrations
            .SelectMany(r => r.Services.OfType<TypedService>().Select(s => s.ServiceType))
            .Distinct();

        foreach (var distinctType in distinctTypes)
        {
            container.Resolve(distinctType);
        }
    }
}