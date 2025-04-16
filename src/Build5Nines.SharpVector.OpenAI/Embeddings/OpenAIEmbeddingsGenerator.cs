using Build5Nines.SharpVector.Embeddings;
using OpenAI.Embeddings;

namespace Build5Nines.SharpVector.OpenAI.Embeddings;

public class OpenAIEmbeddingsGenerator : IEmbeddingsGenerator
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
}