using Autofac;
using Autofac.Extensions.DependencyInjection;
using MailQ.Core.DependencyInjection;
using MailQ.Consumer;
using MailQ.Core.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Project", "MailQ")
    .Enrich.WithProperty("Application", "MailQ.Consumer")
    .WriteTo.GrafanaLoki(EnvironmentVariables.LokiConnectionString 
                         ?? throw new InvalidOperationException("Loki connection string must be set"), 
        createLevelLabel: true)
    .Enrich.WithExceptionDetails()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<MailQModule>();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();