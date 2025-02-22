# Build5Nines SharpVector - a lightweight, in-memory Text Vector Database for any C# / .NET Applications

`Build5Nines.SharpVector` is an in-memory vector database library designed for .NET applications. It allows you to store, search, and manage text data using vector representations. The library is customizable and extensible, enabling support for different vector comparison methods, preprocessing techniques, and vectorization strategies.

Vector databases are used with [Generative AI](https://build5nines.com/what-is-generative-ai/?utm_source=github&utm_medium=sharpvector) solutions augmenting the LLM (Large Language Model) with the ability to load additional context data with the AI prompt using the [RAG (Retrieval-Augmented Generation)](https://build5nines.com/what-is-retrieval-augmented-generation-rag/?utm_source=github&utm_medium=sharpvector) design pattern.

While there are lots of large databases that can be used to build Vector Databases (like Azure CosmosDB, PostgreSQL w/ pgvector, Azure AI Search, Elasticsearch, and more), there are not many options for a lightweight vector database that can be embedded into any .NET application. Build5Nines SharpVector is the lightweight in-memory Text Vector Database for use in any .NET application that you're looking for!

> _"For the in-memory vector database, we're using Build5Nines.SharpVector, an excellent open-source project by Chris Pietschmann. SharpVector makes it easy to store and retrieve vectorized data, making it an ideal choice for our sample RAG implementation."_
>
> — [Tulika Chaudharie, Principal Product Manager at Microsoft for Azure App Service](https://azure.github.io/AppService/2024/09/03/Phi3-vector.html)

## Key Features

- **In-Memory Database**: Lightweight vector database that can be embedded within any .NET application.
- **Vector Comparisons**: Supports various vector comparison methods for searching similar texts.
    - Including cosine similarity (by default), and configurable for Euclidean distance. Or write your own custom vector comparison algorithm.
- **Custom Metadata**: Store additional metadata with each text entry stored in the vector database.
- **Supports async/await**: Async methods for scalable and non-blocking database operations.

## Use Cases

An in-memory vector databases like `Build5Nines.SharpVector` provides several advantages over a traditional vector database server, particularly in scenarios that might demand high performance, low latency, and efficient resource usage.

Here's a list of several usage scenarios where `Build5Nines.SharpVector` can be useful:

- **Development and Testing**: Developers can rapidly prototype and test without the overhead of setting up and managing a server; leading to faster iteration cycles.
- **Small to Medium Scale Applications**: For applications with manageable datasets that fit into memory, in-memory databases offer a simpler and more efficient solution.
- **Low Latency Requirements**: In-memory operations eliminate network latency, providing much lower latency query responses, which is crucial for certain real-time applications.
- **Offline or Edge Computing**: In environments with limited or no internet connectivity, such as remote locations or edge devices, in-memory databases ensure continued functionality.
- **Simplified Deployment**: Integrating an in-memory database directly into a .NET application simplifies deployment and reduces the complexity of the infrastructure.
- **Lower Resource Usage**: In-memory databases can operate with fewer resources, making them ideal for applications with limited compute resources.
- **Experimentation and Research**: In-memory databases can be easier to experiment when trying out new features and algorithms in a controlled and flexible environment.

## Tutorials

Here's a couple helpful tutorial links with additional documentation and examples on using `Build5Nines.SharpVector` in your own projects:

- [Perform Vector Database Similarity Search in .NET Apps using Build5Nines.SharpVector](https://build5nines.com/using-build5nines-sharpvector-for-vector-similarity-search-in-net-applications/?utm_source=github&utm_medium=sharpvector) by Chris Pietschmann
- [Enhanced In-Memory Text Vector Search in .NET with SharpVector and OpenAI Embeddings](https://build5nines.com/enhanced-in-memory-text-vector-search-in-net-with-sharpvector-and-openai-embeddings/?utm_source=github&utm_medium=sharpvector) by Chris Pietschmann
- [Build a Generative AI + RAG App in C# with Phi-3, ONNX, and SharpVector](https://build5nines.com/build-a-generative-ai-rag-app-in-c-with-phi-3-onnx-and-sharpvector/?utm_source=github&utm_medium=sharpvector) by Chris Pietschmann
- [Implementing Local RAG using Phi-3 ONNX Runtime and Sidecar Pattern on Linux App Service](https://azure.github.io/AppService/2024/09/03/Phi3-vector.html) by Tulika Chaudharie (Principal Product Manager at Microsoft for Azure App Service)

## Nuget Package

The `Build5Nines.SharpVector` library is available as a Nuget Package to easily include into your .NET projects:

```bash
dotnet add package Build5Nines.SharpVector
```

You can view it on Nuget.org here: <https://www.nuget.org/packages/Build5Nines.SharpVector/>

## Example Usage

### Loading and Searching Vector Database

As you can see with the following example usage of the `Build5Nines.SharpVector` library, in only a couple lines of code you can embed an in-memory Vector Database into any .NET application:

```csharp
    // Create a Vector Database with metadata of type string
    var vdb = new BasicMemoryVectorDatabase();
    // The Metadata is declared using generics, so you can store whatever data you need there.

    // Load Vector Database with some sample text data
    // Text is the movie description, and Metadata is the movie title with release year in this example
    vdb.AddText("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", "Iron Man (2008)");
    vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "The Lion King (1994)");
    vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", "Alladin (2019)");
    vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", "The Little Mermaid");
    vdb.AddText("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", "Frozen (2013)");

    // Perform a Vector Search
    var result = vdb.Search(newPrompt, pageCount: 5); // return the first 5 results

    if (result.HasResults)
    {
        Console.WriteLine("Similar Text Found:");
        foreach (var item in result.Texts)
        {
            Console.WriteLine(item.Metadata);
            Console.WriteLine(item.Text);
        }
    }
```

The `Build5Nines.SharpVector.BasicMemoryVectorDatabase` class uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor. The library contains generic classes and plenty of extension points to create customized vector database implementations with it if needed.

### Loading with Different Text Chunking Methods

Also, the `TextDataLoader` can be used to help load text documents into the Vector Database with support for multiple different text chunking methods:

```csharp
/// Paragraph Chunking
var loader = new TextDataLoader<int, string>(vdb);
loader.AddDocument(document, new TextChunkingOptions<string>
{
    Method = TextChunkingMethod.Paragraph,
    RetrieveMetadata = (chunk) => {
        // add some basic metadata since this can't be null
        return "{ chuckSize: \"" + chunk.Length + "\" }";
    }
});

/// Sentence Chunking
var loader = new TextDataLoader<int, string>(vdb);
loader.AddDocument(document, new TextChunkingOptions<string>
{
    Method = TextChunkingMethod.Sentence,
    RetrieveMetadata = (chunk) => {
        // add some basic metadata since this can't be null
        return "{ chuckSize: \"" + chunk.Length + "\" }";
    }
});

/// Fixed Length Chunking
var loader = new TextDataLoader<int, string>(vdb);
loader.AddDocument(document, new TextChunkingOptions<string>
{
    Method = TextChunkingMethod.FixedLength,
    ChunkSize = 150,
    RetrieveMetadata = (chunk) => {
        // add some basic metadata since this can't be null
        return "{ chuckSize: \"" + chunk.Length + "\" }";
    }
});
```

The `RetrieveMetadata` accepts a lambda function that can be used to easily define the Metadata for the chucks as they are loaded.

## Sample Console App

The [sample console app](src/ConsoleTest/) in this repo show example usage of Build5Nines.SharpVector.dll

It loads a list of movie titles and descriptions from a JSON file, then allows the user to type in prompts to search the database and return the best matches.

Here's a screenshot of the test console app running:

![](assets/build5nines-sharpvector-console-screenshot.jpg)

## Maintained By

The **Build5Nines SharpVector** project is maintained by [Chris Pietschmann](https://pietschsoft.com?utm_source=github&utm_medium=sharpvector), founder of [Build5Nines](https://build5nines.com?utm_source=github&utm_medium=sharpvector), Microsoft MVP, HashiCorp Ambassador, and Microsoft Certified Trainer (MCT).
