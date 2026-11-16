namespace Core.Reports;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TopBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, int> primarySelector,
        Func<T, TKey> tieBreakerSelector,
        int count,
        IComparer<TKey>? tieBreakerComparer = null)
    {
        return source
            .OrderByDescending(primarySelector)
            .ThenBy(tieBreakerSelector, tieBreakerComparer)
            .Take(count);
    }
}