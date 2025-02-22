namespace Build5Nines.SharpVector;

public static class IVectorDatabaseExtensions
{
    public static async Task SaveToFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filename)
        where TId : notnull
    {
        using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            await vectorDatabase.SerializeToJsonStreamAsync(stream);
        }
    }

    public static void SaveToFile<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filename)
        where TId : notnull
    {
        using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            vectorDatabase.SerializeToJsonStream(stream);
        }
    }

    public static async Task LoadFromFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filename)
        where TId : notnull
    {
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            await vectorDatabase.DeserializeFromJsonStreamAsync(stream);
        }
    }

    public static void LoadFromFile<TId, TMetadata, TDocument>(IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filename)
        where TId : notnull
    {
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            vectorDatabase.DeserializeFromJsonStream(stream);
        }
    }
}