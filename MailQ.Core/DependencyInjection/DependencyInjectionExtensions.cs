using Autofac;
using Autofac.Builder;

namespace MailQ.Core.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static void RegisterSingleton<TInterface, TImplementation>(this ContainerBuilder builder) 
        where TImplementation : notnull 
        where TInterface : notnull
    {
        builder.RegisterType<TImplementation>().As<TInterface>().SingleInstance();
    }

    public static void RegisterScoped<TInterface, TImplementation>(this ContainerBuilder builder) 
        where TImplementation : notnull 
        where TInterface : notnull
    {
        builder.RegisterType<TImplementation>().As<TInterface>().InstancePerLifetimeScope();
    }
}