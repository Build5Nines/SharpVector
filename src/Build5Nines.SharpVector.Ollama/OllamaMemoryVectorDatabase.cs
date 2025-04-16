using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Embeddings;

namespace Build5Nines.SharpVector.Ollama;

/// <summary>
/// An interface for a vector database that uses OpenAI for embedding generation.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public interface IOllamaMemoryVectorDatabase<TId, TMetadata> : IVectorDatabase<TId, TMetadata>
    where TId : notnull
{ }

/// <summary>
/// A simple in-memory database for storing and querying vectorized text items.
/// This database uses OpenAI to generate embeddings, and performs Cosine similarity search.
/// </summary>
/// <typeparam name="TMetadata">Defines the data type for the Metadata stored with the Text.</typeparam>
public class OllamaMemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int,
        TMetadata,
        MemoryDictionaryVectorStore<int, TMetadata>,
        IntIdGenerator,
        CosineSimilarityVectorComparer
        >, IOllamaMemoryVectorDatabase<int, TMetadata>
{
    public OllamaMemoryVectorDatabase(string ollamaEndpoint, string model)
        : this(
            new OllamaEmbeddingsGenerator(ollamaEndpoint, model)
            )
    { }

    public OllamaMemoryVectorDatabase(IEmbeddingsGenerator embeddingsGenerator)
        : base(
            embeddingsGenerator,
            new MemoryDictionaryVectorStore<int, TMetadata>()
            )
    { }
}
