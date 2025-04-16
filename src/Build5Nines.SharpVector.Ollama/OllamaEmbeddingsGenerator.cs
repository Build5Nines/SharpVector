using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Build5Nines.SharpVector.Embeddings;

namespace Build5Nines.SharpVector.Ollama;

public class OllamaEmbeddingsGenerator : IEmbeddingsGenerator
{
    public string Model { get; set; }

    public string Endpoint { get; set; }

    public OllamaEmbeddingsGenerator(string ollamaEndpoint, string model)
    {
        Endpoint = ollamaEndpoint;
        Model = model;
    }

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