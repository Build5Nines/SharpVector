using System.Text;

namespace Build5Nines.SharpVector;

public static class IVectorDatabaseExtensions
{
    public static async Task SaveToFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await vectorDatabase.SerializeToJsonStreamAsync(stream);
        }
    }

    public static void SaveToFile<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            vectorDatabase.SerializeToJsonStream(stream);
        }
    }

    public static async Task LoadFromFileAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await vectorDatabase.DeserializeFromJsonStreamAsync(stream);
            await stream.FlushAsync();
        }
    }

    public static void LoadFromFile<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string filePath)
        where TId : notnull
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            vectorDatabase.DeserializeFromJsonStream(stream);
            stream.Flush();
        }
    }

    public static async Task<string> SerializeToJsonAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase)
    {
        using (var stream = new MemoryStream())
        {
            await vectorDatabase.SerializeToJsonStreamAsync(stream);

            var stringReader = new StreamReader(stream, Encoding.UTF8);
            stream.Position = 0;
            var json = await stringReader.ReadToEndAsync();
            return json;
        }
    }

    public static string SerializeToJson<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase)
    {
        var task = vectorDatabase.SerializeToJsonAsync();
        task.Wait();
        return task.Result;
    }

    public static async Task DeserializeFromJsonAsync<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string json)
    {
        using (var stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream, Encoding.UTF8);
            await writer.WriteAsync(json);
            await writer.FlushAsync(); // Ensure all data is written to the stream
            stream.Position = 0; // Reset the stream position to the beginning
            await vectorDatabase.DeserializeFromJsonStreamAsync(stream);
        }
    }

    public static void DeserializeFromJson<TId, TMetadata, TDocument>(this IVectorDatabase<TId, TMetadata, TDocument> vectorDatabase, string json)
        where TId : notnull
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(json);
                writer.Flush(); // Ensure all data is written to the stream
                stream.Position = 0; // Reset the stream position to the beginning
                vectorDatabase.DeserializeFromJsonStream(stream);
            }
        }
    }
}