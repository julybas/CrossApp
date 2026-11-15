using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

// 1. Ініціалізуємо сховище та сервіс із даними з SampleData
ICatalogStore store = new InMemoryCatalogStore(SampleData.Products());
var service = new CatalogService(store);

var items = service.All();
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.TopByQuantity(items, 5));
        break;
    case "report-low":
        Print(ReportService.LowStock(items, 10));
        break;
    case "report-groups":
        foreach (var g in ReportService.ByUnit(items))
            Console.WriteLine($"{g.Key,-6} позицій: {g.Count,3}   сумарно: {g.TotalQuantity,6}");
        break;
    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"позицій {t.Items}, всього {t.Total}, середнє {t.Average:F2}, макс {t.Max}");
        break;
    case "demo-deferred":
        RunDeferredDemo(service);
        break;
    default:
        Console.WriteLine("Доступні команди: report-top | report-low | report-groups | report-sum | demo-deferred");
        break;
}

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($"{l.Sku,-10} {l.Name,-25} {l.Quantity,6} {l.Unit}");
}

static void RunDeferredDemo(CatalogService service)
{
    var source = service.All().ToList();
    var query = source.Where(p => p.Quantity < 10);

    Console.WriteLine($"Дефіцитних до змін: {query.Count()}");

    source.Add(Product.Create("tmp00001", "SKU-999", "Скоба", "шт", 3));
    Console.WriteLine($"Дефіцитних після змін: {query.Count()}");

    var snapshot = query.ToList();
    source.Clear();
    Console.WriteLine($"У знімку лишилось: {snapshot.Count}");
}