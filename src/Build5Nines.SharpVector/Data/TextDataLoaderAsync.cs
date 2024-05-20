namespace Build5Nines.SharpVector.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class TextDataLoaderAsync<TId, TMetadata> : TextDataLoader<TId, TMetadata>
    where TId : notnull
    where TMetadata : notnull
{
    public TextDataLoaderAsync(IVectorDatabaseAsync<TId, TMetadata> vectorDatabase)
        : base(vectorDatabase)
    {
        vectorDatabaseAsync = vectorDatabase;
    }

    private IVectorDatabaseAsync<TId, TMetadata> vectorDatabaseAsync;

    public async Task<IEnumerable<TId>> AddDocumentAsync(string document, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        if (chunkingOptions.RetrieveMetadata == null)
            throw new ValidationException("TextChunkingOptions.RetrieveMetadata must be set");

        var chunks = await ChunkTextAsync(document, chunkingOptions);
        var ids = new List<TId>();
        object _lock = new object();
        await Parallel.ForEachAsync(chunks, async (chunk, cancellationToken) =>
        {
            var id = await vectorDatabaseAsync.AddTextAsync(chunk, chunkingOptions.RetrieveMetadata.Invoke(chunk));
            lock (_lock) {
                ids.Add(id);
            }
        });

        return ids;
    }

    private async Task<List<string>> ChunkTextAsync(string text, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        return await Task.Run(() => ChunkText(text, chunkingOptions));
    }
}
