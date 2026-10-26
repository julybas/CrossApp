using Core.Abstractions;

namespace Core.Storage;

public static class StoreFactory
{
    public static ICatalogStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        
        if (useFile)
        {
            string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
            return new FileCatalogStore(dataPath);
        }

        return new InMemoryCatalogStore(SampleData.Products());
    }
}