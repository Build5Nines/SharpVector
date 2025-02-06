Build5Nines.SharpVector is a lightweight in-memory Vector Database for use in any .NET application.

The `Build5Nines.SharpVector.BasicMemoryVectorDatabase` class uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.

### Example Usage: Load and Search Vector Database

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

### Example Usage: Loading with Different Text Chunking Methods

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
```

## Tutorials

Here's a couple helpful tutorial links with additional documentation and examples on using `Build5Nines.SharpVector` in your own projects:

- [Perform Vector Database Similarity Search in .NET Apps using Build5Nines.SharpVector](https://build5nines.com/using-build5nines-sharpvector-for-vector-similarity-search-in-net-applications/) by Chris Pietschmann
- [Build a Generative AI + RAG App in C# with Phi-3, ONNX, and SharpVector](https://build5nines.com/build-a-generative-ai-rag-app-in-c-with-phi-3-onnx-and-sharpvector/) by Chris Pietschmann
- [Implementing Local RAG using Phi-3 ONNX Runtime and Sidecar Pattern on Linux App Service](https://azure.github.io/AppService/2024/09/03/Phi3-vector.html) by Tulika Chaudharie (Principal Product Manager at Microsoft for Azure App Service)
