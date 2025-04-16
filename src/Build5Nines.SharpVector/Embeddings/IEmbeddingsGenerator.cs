namespace Build5Nines.SharpVector.Embeddings;

public interface IEmbeddingsGenerator
{
    Task<float[]> GenerateEmbeddingsAsync(string text);
}