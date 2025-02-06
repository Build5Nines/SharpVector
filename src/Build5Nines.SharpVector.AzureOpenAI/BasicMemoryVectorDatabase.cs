using OpenAI.Embeddings;

namespace Build5Nines.SharpVector.AzureOpenAI;

/// <summary>
/// A basic implementation of an vector database that uses an in-memory dictionary to store vectors generated using the specified OpenAI embeddings client, with integer keys and string metadata values.
/// </summary>
public class BasicMemoryVectorDatabase : MemoryVectorDatabase<string>
{
    public BasicMemoryVectorDatabase(EmbeddingClient embeddingClient)
        : base(embeddingClient)
    { }
}