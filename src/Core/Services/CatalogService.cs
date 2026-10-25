using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService(ICatalogStore store, INotificationSink? sink = null)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly INotificationSink? _sink = sink;

    private void Notify(Product p, CatalogChangeKind kind, int delta) =>
        _sink?.Notify(new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity));

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var product = Product.Create(id, sku, name, unit, quantity);
        _store.Add(product);

        Notify(product, CatalogChangeKind.Added, quantity);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");

        product.RegisterArrival(quantity);
        _store.Update(product);

        Notify(product, CatalogChangeKind.Received, quantity);
    }
}