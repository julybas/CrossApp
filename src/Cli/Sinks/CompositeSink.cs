using Core.Abstractions;
using Core.Events;

namespace Cli.Sinks;

public sealed class CompositeSink(IEnumerable<INotificationSink> sinks) : INotificationSink
{
    private readonly List<INotificationSink> _sinks = sinks.ToList();

    public void Notify(CatalogChangedEventArgs e)
    {
        foreach (var sink in _sinks)
        {
            sink.Notify(e);
        }
    }
}