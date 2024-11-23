# Build5Nines SharpVector - a lightweight, in-memory Vector Database for any C# / .NET Applications

`Build5Nines.SharpVector` is an in-memory vector database library designed for .NET applications. It allows you to store, search, and manage text data using vector representations. The library is customizable and extensible, enabling support for different vector comparison methods, preprocessing techniques, and vectorization strategies.

Vector databases are used with Generative AI solutions augmenting the LLM (Large Language Model) with the ability to load additional context data with the AI prompt using the RAG (Retrieval-Augmented Generation) design pattern.

While there are lots of large databases that can be used to build Vector Databases (like Azure CosmosDB, PostgreSQL w/ pgvector, Azure AI Search, Elasticsearch, and more), there are not many options for a lightweight vector database that can be embedded into any .NET application.

The Build5Nines SharpVector project provides a lightweight in-memory Vector Database for use in any .NET application.

> _"An excellent open-source project by Chris Pietschmann. SharpVector makes it easy to store and retrieve vectorized data, making it an ideal choice for our sample RAG implementation."_
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

## Nuget Package

The `Build5Nines.SharpVector` library is available as a Nuget Package to easily include into your .NET projects:

```bash
dotnet add package Build5Nines.SharpVector
```

You can view it on Nuget.org here: <https://www.nuget.org/packages/Build5Nines.SharpVector/>

## Supports .NET 6 and greater

The library is built using no external dependencies other than what's available from .NET, and it's built to target .NET 6 and greater.

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

## Change Log

### v1.0.0 (2024-05-24)

- Simplify object model by combining Async and non-Async classes, `BasicMemoryVectorDatabase` now support both synchronous and asynchronous operations.
- Refactored to remove unnecessary classes where the `Async` versions will work just fine.
- Improve async/await and multi-threading use

### v0.9.8-beta (2024-05-20)

- Added `Async` version of classes to support multi-threading
- Metadata is no longer required when calling `.AddText()` and `.AddTextAsync()`
- Refactor `IVectorSimilarityCalculator` to `IVectorComparer` and `CosineVectorSimilarityCalculatorAsync` to `CosineSimilarityVectorComparerAsync`
- Add new `EuclideanDistanceVectorComparerAsync`
- Fix `MemoryVectorDatabase` to no longer requird unused `TId` generic type
- Rename `VectorSimilarity` and `Similarity` properties to `VectorComparison`

### v0.9.5-beta (2024-05-18)

- Add `TextDataLoader` class to provide support for different methods of text chunking when loading documents into the vector database.

### v0.9.0-beta (2024-05-18)

- Introduced the `BasicMemoryVectorDatabase` class as the basic Vector Database implementations that uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.
- Add more C# Generics use, so the library is more customizable when used, and custom vector databases can be implemented if desired.
- Added `VectorTextResultItem.Similarity` so consuming code can inspect similarity of the Text in the vector search results.
- Update `.Search` method to support search result paging and threshold support for similarity comparison
- Add some basic Unit Tests

### v0.8.0-beta (2024-05-17)

- Initial release - let's do this!

## Maintained By

The [Build5Nines](https://build5nines.com) SharpVector project is maintained by [Chris Pietschmann](https://pietschsoft.com), Microsoft MVP, HashiCorp Ambassador.
