using Core.Abstractions;
using Core.Events;

namespace Cli.Sinks;

public sealed class ConsoleSink : INotificationSink
{
    public void Notify(CatalogChangedEventArgs e)
    {
        var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
        Console.WriteLine(line);
    }
}