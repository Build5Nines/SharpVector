namespace Build5Nines.SharpVector.Embeddings;

/// <summary>
/// Optional capability for embeddings generators to support batch embedding of multiple texts.
/// Implementations can leverage provider APIs that accept multi-input requests for better performance.
/// </summary>
public interface IBatchEmbeddingsGenerator : IEmbeddingsGenerator
{
    /// <summary>
    /// Generates embeddings for multiple input texts in a single call when supported.
    /// </summary>
    /// <param name="texts">Collection of texts to embed. Order should be preserved in output.</param>
    /// <returns>A read-only list of embeddings vectors corresponding to the input order.</returns>
    Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts);
}
