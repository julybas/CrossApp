using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    public static IReadOnlyList<StockLine> LowStock(IEnumerable<Product> source, int threshold) =>
        source.Where(p => p.Quantity < threshold)
              .OrderBy(p => p.Quantity)
              .ThenBy(p => p.Sku, StringComparer.Ordinal)
              .Select(p => new StockLine(p.Sku, p.Name, p.Quantity, p.Unit))
              .ToList();

    public static IReadOnlyList<StockLine> TopByQuantity(IEnumerable<Product> source, int n) =>
        source.TopBy(p => p.Quantity, p => p.Sku, n, StringComparer.Ordinal)
              .Select(p => new StockLine(p.Sku, p.Name, p.Quantity, p.Unit))
              .ToList();

    public static IReadOnlyList<GroupSummary> ByUnit(IEnumerable<Product> source) =>
        source.GroupBy(p => p.Unit, StringComparer.OrdinalIgnoreCase)
              .Select(g => new GroupSummary(g.Key, g.Count(), g.Sum(p => p.Quantity)))
              .OrderByDescending(s => s.TotalQuantity)
              .ToList();

    public static IReadOnlyList<GroupSummary> TopUnitsByQuantity(IEnumerable<Product> source, int n) =>
        ByUnit(source)
            .TopBy(g => g.TotalQuantity, g => g.Key, n, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public static IReadOnlyList<GroupSummary> ByUnitQuerySyntax(IEnumerable<Product> source)
    {
        var query =
            from product in source
            group product by product.Unit.ToUpperInvariant() into g
            let total = g.Sum(p => p.Quantity)
            orderby total descending
            select new GroupSummary(g.Key, g.Count(), total);

        return query.ToList();
    }

    public static (int Items, int Total, double Average, int Max) Totals(IEnumerable<Product> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (list.Count,
               list.Sum(p => p.Quantity),
               list.Average(p => p.Quantity),
               list.Max(p => p.Quantity));
    }
}