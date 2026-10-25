using Core.Abstractions;
using Core.Events;

namespace Cli.Sinks;

public sealed class FileSink(string filePath) : INotificationSink
{
    public void Notify(CatalogChangedEventArgs e)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
        File.AppendAllText(filePath, line + Environment.NewLine);
    }
}