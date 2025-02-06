using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using OpenAI.Embeddings;

namespace Build5Nines.SharpVector.OpenAI;

/// <summary>
/// A simple in-memory database for storing and querying vectorized text items.
/// This database uses OpenAI to generate embeddings, and performs Cosine similarity search.
/// </summary>
/// <typeparam name="TMetadata">Defines the data type for the Metadata stored with the Text.</typeparam>
public class OpenAIMemoryVectorDatabase<TMetadata>
     : OpenAIMemoryVectorDatabaseBase<
        int,
        TMetadata,
        MemoryDictionaryVectorStore<int, TMetadata>,
        IntIdGenerator,
        CosineSimilarityVectorComparer
        >
{
    public OpenAIMemoryVectorDatabase(EmbeddingClient embeddingClient)
        : base(
            embeddingClient,
            new MemoryDictionaryVectorStore<int, TMetadata>()
            )
    { }
}
