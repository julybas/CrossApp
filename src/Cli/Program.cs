using System.Text;
using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// --- Composition Root (Створення сховища через фабрику та ініціалізація сервісу) ---
ICatalogStore store = StoreFactory.Create(args);
var service = new CatalogService(store);

Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
Console.WriteLine(store is FileCatalogStore 
    ? "Режим: файлове сховище (data/catalog.json)\n" 
    : "Режим: у пам'яті (скинеться після завершення)\n");

// --- Сценарій 1: Додавання та операції через сервіс ---
Console.WriteLine("=== Сценарій 1: Робота через CatalogService ===");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
Console.WriteLine($"Додано: {created}");

service.Receive(created.Id, 25);
Console.WriteLine($"Після приходу (+25): {service.Find(created.Id)}");

service.Issue(created.Id, 30);
Console.WriteLine($"Після видачі (-30):  {service.Find(created.Id)}");
Console.WriteLine();

// --- Сценарій 2: Виведення списку товарів зі сховища ---
Console.WriteLine("=== Сценарій 2: Список усіх товарів (перші 5) ===");
foreach (var p in service.All().Take(5))
{
    Console.WriteLine($"  {p.Id,-8} {p.Sku,-10} {p.Name,-25} {p.Quantity,5} {p.Unit}");
}
Console.WriteLine($"Всього в сховищі: {service.All().Count} товарів.\n");

// --- Сценарій 3: Перевірка інваріантів та сервісних помилок ---
Console.WriteLine("=== Сценарій 3: Порушення інваріантів та помилки ===");
TryDo("Видача більша за залишок (через сервіс)", () => service.Issue(created.Id, 1000));
TryDo("Порожній SKU (створення сутності)", () => Product.Create("P-002", "", "Пісок", "т", 10));
TryDo("Спроба операції з неіснуючим ID", () => service.Receive("NON-EXISTENT-ID", 10));

Console.WriteLine("\n=== Додаткове завдання: Пошук за умовою (Func<Product, bool>) ===");
var lowStock = service.Find(p => p.Quantity < 50);
Console.WriteLine($"Товари із залишком менше 50 шт ({lowStock.Count}):");
foreach (var p in lowStock)
{
    Console.WriteLine($"   {p.Sku,-10} {p.Name,-25} {p.Quantity,5} {p.Unit}");
}

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"   {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   {title}: {ex.GetType().Name} — {ex.Message}");
    }
}