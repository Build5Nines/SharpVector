using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Ollama;


Console.WriteLine("Test OllamaEmbeddingsGenerator");

var generator = new OllamaEmbeddingsGenerator("http://localhost:11434/api/embeddings", "nomic-embed-text");
var embeddings = await generator.GenerateEmbeddingsAsync("Hello World");

foreach (var item in embeddings)
{
    Console.Write(item + ", ");
}
Console.WriteLine("");

Console.WriteLine("Test BasicOllamaMemoryVectorDatabase");

var vdb = new BasicOllamaMemoryVectorDatabase("http://localhost:11434/api/embeddings", "nomic-embed-text");

vdb.AddText("Hello World", "metadata");
vdb.AddText("Hola", "metadata2");

var result = vdb.Search("Hola Senior");

foreach (var item in result.Texts)
{
    Console.WriteLine($"{item.Text} - {item.Metadata} - {item.VectorComparison}");
}
