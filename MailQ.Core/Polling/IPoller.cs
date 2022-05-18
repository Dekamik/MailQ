namespace MailQ.Core.Polling;

public interface IPoller
{
    Task DoPolling(CancellationToken cancellationToken);
}