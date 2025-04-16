using Build5Nines.SharpVector.Embeddings;

namespace Build5Nines.SharpVector.Ollama;

/// <summary>
/// A basic implementation of an vector database that uses an in-memory dictionary to store vectors generated using the specified OpenAI embeddings client, with integer keys and string metadata values.
/// </summary>
public class BasicOllamaMemoryVectorDatabase : OllamaMemoryVectorDatabase<string>
{
    public BasicOllamaMemoryVectorDatabase(string model)
        : this(
            new OllamaEmbeddingsGenerator(model)
            )
    { }

    public BasicOllamaMemoryVectorDatabase(string ollamaEndpoint, string model)
        : this(
            new OllamaEmbeddingsGenerator(ollamaEndpoint, model)
            )
    { }
    
    public BasicOllamaMemoryVectorDatabase(IEmbeddingsGenerator embeddingsGenerator)
        : base(embeddingsGenerator)
    { }
}