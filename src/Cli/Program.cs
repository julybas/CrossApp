using System.Text;
using Core.Domain;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: успіх ===");
Product product = Product.Create("P-001", "sku-001", "Цемент М400 25кг", "шт", 100);
Console.WriteLine(product);

product.RegisterArrival(50);
product.Issue(30);
Console.WriteLine(product);

Console.WriteLine("\n=== Сценарій 2: порушення інваріантів ===");
TryDo("видача більша за залишок", () => product.Issue(1000));
TryDo("порожній SKU", () => Product.Create("P-002", "", "Пісок", "т", 10));
TryDo("від'ємний залишок", () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

Console.WriteLine("\n=== Сценарій 3 (Додаткове): Імпорт з CSV + валідація інваріантів ===");
string path = Path.Combine("data", "sample.csv");
if (File.Exists(path))
{
    ImportResult<ProductDto> csvResult = ProductCsvImporter.Load(path);
    var (entities, domainErrors) = Product.FromImportResult(csvResult);

    Console.WriteLine($"Створено валідних доменних сутностей: {entities.Count}");
    foreach (var p in entities.Take(3))
    {
        Console.WriteLine($"  {p}");
    }

    if (domainErrors.Count > 0)
    {
        Console.WriteLine($"Помилки доменних інваріантів під час імпорту: {domainErrors.Count}");
        foreach (var err in domainErrors)
        {
            Console.WriteLine($"  ! {err}");
        }
    }
}

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
    }
}