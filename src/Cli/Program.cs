using Cli.Sinks;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

ICatalogStore store = new InMemoryCatalogStore();

// Формуємо шлях до файлу логів
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");

// Додаткове завдання: Створюємо CompositeSink з двох Sink-ів
var sink = new CompositeSink(new INotificationSink[]
{
    new ConsoleSink(),
    new FileSink(logPath)
});

// Передаємо sink у сервіс (ручний DI)
var service = new CatalogService(store, sink);

// Крок 3: Локальна функція-підписник для подій C#
void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    var line = $"[Event] {e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    Console.WriteLine(line);
}

// Підписка іменованим методом (щоб потім можна було відписатися)
service.Changed += OnCatalogChanged;

// Крок 4: Другий підписник — лямбда із замиканням над лічильником
int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("=== 1–5. Успішні операції та підписка ===");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
service.Receive(created.Id, 25);
Console.WriteLine($"Подій отримано: {changes}\n");

Console.WriteLine("=== Відписка від події (service.Changed -= OnCatalogChanged) ===");
service.Changed -= OnCatalogChanged;
service.Receive(created.Id, 5); // Операція виконається, але event-обробник вже не виведе рядок
Console.WriteLine($"Подій отримано (лямбда продовжила рахувати): {changes}\n");

Console.WriteLine("=== Крок 6. Сценарій відмови (інваріант) ===");
try
{
    // Некоректне значення (-10) викликає виняток інваріанту в сервісі
    service.Receive(created.Id, -10); 
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка (очікувано): {ex.Message}");
}
Console.WriteLine($"Подій після помилки: {changes}\n"); // Лічильник не змінився!

Console.WriteLine("=== Крок 7. Делегат Func (пошук товарів) ===");
// Використовуємо предикат p.Quantity < 100 для відбору товарів
var filteredProducts = service.FindAll(p => p.Quantity < 100);
foreach (var p in filteredProducts)
{
    Console.WriteLine($"Знайдено: {p.Name} | Залишок: {p.Quantity}");
}