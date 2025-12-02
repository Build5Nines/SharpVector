using Build5Nines.SharpVector.Embeddings;
using OpenAI.Embeddings;
using System.Collections.Generic;
using System.Linq;

namespace Build5Nines.SharpVector.OpenAI.Embeddings;

public class OpenAIEmbeddingsGenerator :  IBatchEmbeddingsGenerator
{
    protected EmbeddingClient EmbeddingClient { get; private set; }

    public OpenAIEmbeddingsGenerator(EmbeddingClient embeddingClient)
    {
        EmbeddingClient = embeddingClient;
    }
    public async Task<float[]> GenerateEmbeddingsAsync(string text)
    {
        var result = await EmbeddingClient.GenerateEmbeddingAsync(text);
        var embedding = result.Value;
        var vector = embedding.ToFloats();
        return vector.ToArray();
    }

    /// <summary>
    /// Generates embeddings for a batch of input texts using the OpenAI embeddings client.
    /// This leverages the API's multi-input batching for improved throughput and reduced overhead.
    /// </summary>
    /// <param name="texts">Collection of non-empty texts to embed.</param>
    /// <returns>A list of float vectors aligned to the input order.</returns>
    public async Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts)
    {
        if (texts is null) throw new ArgumentNullException(nameof(texts));

        var inputs = texts.ToList();
        if (inputs.Count == 0)
        {
            return Array.Empty<float[]>();
        }

        // Call the batch embeddings API once for all inputs.
        var batchResult = await EmbeddingClient.GenerateEmbeddingsAsync(inputs);

        // Map the embeddings to float arrays while preserving order.
        var vectors = batchResult.Value.Select(e => e.ToFloats().ToArray()).ToList();

        return vectors;
    }
}