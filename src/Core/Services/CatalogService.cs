using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService(ICatalogStore store, INotificationSink? sink = null)
{
    private readonly ICatalogStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly INotificationSink? _sink = sink;

    // 1. Подія згідно з основним завданням ЛР6
    public event EventHandler<CatalogChangedEventArgs>? Changed;

    private void OnChanged(Product p, CatalogChangeKind kind, int delta)
    {
        var args = new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity);
        
        // Виклик події для підписників (+=)
        Changed?.Invoke(this, args);
        
        // Виклик додаткового завдання (Sink)
        _sink?.Notify(args);
    }

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        var product = Product.Create(id, sku, name, unit, quantity);
        _store.Add(product);

        OnChanged(product, CatalogChangeKind.Added, quantity);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");

        product.RegisterArrival(quantity);
        _store.Update(product);

        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id) 
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");

        product.Issue(quantity);
        _store.Update(product);

        OnChanged(product, CatalogChangeKind.Issued, -quantity);
    }

    public bool Delete(string id)
    {
        var product = _store.GetById(id);
        if (product is null) return false;

        var result = _store.Remove(id);
        if (result)
        {
            OnChanged(product, CatalogChangeKind.Removed, 0);
        }

        return result;
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    public IReadOnlyList<Product> Find(Func<Product, bool> predicate) => _store.Search(predicate);

    // Метод FindAll для Кроку 7
    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate) => 
        _store.List().Where(predicate).ToList();
}