using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Build5Nines.SharpVector.Embeddings;

namespace Build5Nines.SharpVector.Ollama.Embeddings;

public class OllamaEmbeddingsGenerator : IEmbeddingsGenerator
{
    public string Model { get; set; }

    public string Endpoint { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="OllamaEmbeddingsGenerator"/> class.
    /// This constructor uses the default Ollama embeddings endpoint URL.
    /// </summary>
    /// <param name="model">Ollama embeddings model</param>
    public OllamaEmbeddingsGenerator(string model)
        : this("http://localhost:11434/api/embeddings", model)
    { }

    /// <summary>
    /// Creates a new instance of the <see cref="OllamaEmbeddingsGenerator"/> class.
    /// </summary>
    /// <param name="ollamaEndpoint">Ollama embeddings endpoint URL.</param>
    /// <param name="model">Ollama embeddings model</param>
    public OllamaEmbeddingsGenerator(string ollamaEndpoint, string model)
    {
        Endpoint = ollamaEndpoint;
        Model = model;
    }

    /// <summary>
    /// Generates embeddings for the given text using the specified Ollama model.
    /// </summary>
    /// <param name="text">The text to generate embeddings for.</param>
    /// <returns>An array of floats representing the generated embeddings.</returns>
    public async Task<float[]> GenerateEmbeddingsAsync(string text)
    {
        var requestBody = new
        {
            model = Model,
            prompt = text
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(Endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var embeddingResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseString);

        return embeddingResponse?.Embedding ?? Array.Empty<float>();
    }

    private class OllamaEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
    }
}