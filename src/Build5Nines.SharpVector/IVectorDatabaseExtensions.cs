using System.Text;

namespace Build5Nines.SharpVector;

public static class IVectorDatabaseExtensions
{
    public static async Task SaveToFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await vectorDatabase.SerializeToBinaryStreamAsync(stream);
        }
    }

    public static void SaveToFile<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            vectorDatabase.SerializeToBinaryStream(stream);
        }
    }

    public static async Task LoadFromFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await vectorDatabase.DeserializeFromBinaryStreamAsync(stream);
        }
    }

    public static void LoadFromFile<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            vectorDatabase.DeserializeFromBinaryStream(stream);
        }
    }
}