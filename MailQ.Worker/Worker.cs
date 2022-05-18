using MailQ.Core.Polling;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace MailQ.Worker;

public class Worker : BackgroundService
{
    private readonly IPoller _poller;

    public Worker(IPoller poller)
    {
        _poller = poller;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _poller.DoPolling(stoppingToken);
        }
        catch (BrokerUnreachableException ex)
        {
            Log.Fatal(ex, "Broker unreachable at {Uri}", 
                EnvironmentVariables.RabbitMqConnectionString);
            throw;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled error occurred");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}