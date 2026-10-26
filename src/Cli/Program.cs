using Cli.Sinks;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

ICatalogStore store = new InMemoryCatalogStore();

// Формуємо шлях та створюємо директорію ДО виконання будь-яких операцій
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
var logDir = Path.GetDirectoryName(logPath);
if (!string.IsNullOrEmpty(logDir))
{
    Directory.CreateDirectory(logDir);
}

var sink = new CompositeSink(new INotificationSink[]
{
    new ConsoleSink(),
    new FileSink(logPath)
});

var service = new CatalogService(store, sink);

void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    Console.WriteLine(line);
    
    // Перевірка на випадок, якщо директорію видалили під час роботи
    if (!string.IsNullOrEmpty(logDir))
    {
        Directory.CreateDirectory(logDir);
    }
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnCatalogChanged;

int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("=== 1. Успішні операції ===");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
service.Receive(created.Id, 25);
Console.WriteLine($"Подій отримано: {changes}\n");

Console.WriteLine("=== 2. Відписка від події ===");
service.Changed -= OnCatalogChanged;
service.Receive(created.Id, 5);
Console.WriteLine($"Подій отримано: {changes}\n");

Console.WriteLine("=== 3. Сценарій відмови (інваріант) ===");
try
{
    service.Receive(created.Id, -10);
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}
Console.WriteLine($"Подій після помилки: {changes}\n");

Console.WriteLine("=== 4. Фільтрація через Func ===");
var lowStock = service.FindAll(p => p.Quantity < 100);
foreach (var product in lowStock)
{
    Console.WriteLine($"Товар: {product.Name}, Залишок: {product.Quantity}\n");
}

Console.WriteLine("=== Крок 6. Сценарій відмови (інваріант) ===");
try
{
    // Некоректне значення: -10 викликає виняток інваріанту в RegisterArrival
    service.Receive(created.Id, -10); 
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка (очікувано): {ex.Message}");
}
Console.WriteLine($"Подій після помилки: {changes}\n"); // Лічильник не змінився!

Console.WriteLine("=== Крок 7. Делегат Func (пошук товарів) ===");
// Використовуємо предикат p.Quantity < 100, щоб створений кабель (75 шт) потрапив у результат
var filteredProducts = service.FindAll(p => p.Quantity < 100);
foreach (var p in filteredProducts)
{
    Console.WriteLine($"Знайдено: {p.Name} | Залишок: {p.Quantity}");
}