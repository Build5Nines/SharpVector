# Build5Nines SharpVector is a simple in-memory Vector Database for C# Applications

Vector databases are used with Generative AI solutions augmenting the LLM (Large Language Model) with the ability to load additional context data with the AI prompt using the RAG (Retrieval-Augmented Generation) design pattern.

While there are lots of large databases that can be used to build Vector Databases (like Azure CosmosDB, PostgreSQL w/ pgvector, Azure AI Search, Elasticsearch, and more), there aren't many options for a light weight, simple solution to embed into any .NET application.

The Build5Nines SharpVector project provides a simple, small, easy to embed, in-memory Vector Database for use in any .NET application.

The vector algorithm currently used in the `Build5Nines.SharpVector.MemoryVectorDatabase` class is based on TF-IDF (Term Frequency-Inverse Document Frequency) combined with cosine similarity for searching similar texts.

## Nuget Package

The `Build5Nines.SharpVector` library is available as a Nuget Package to easily include into your .NET projects:

```bash
dotnet add package Build5Nines.SharpVector --version 0.8.0-beta
```

You can view it on Nuget.org here: <https://www.nuget.org/packages/Build5Nines.SharpVector/>

## Supports .NET 6 and greater

The library is built using no external dependencies other than what's available from .NET, and it's built to target .NET 6 and greater.

## Example Usage

As you can see with the following example usage of the `Build5Nines.SharpVector` library, it's really simple to embed an in-memory Vector Database for use in any .NET application:

```csharp
    // Create a Vector Database with metadata of type string
    IVectorDatabase<string> vdb = new MemoryVectorDatabase<string>();
    // The Metadata is declared using generics, so you can store whatever data you need there.

    // Load Vector Database with some sample text data
    // Text is the movie description, and Metadata is the movie title with release year in this example
    vdb.AddText("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", "Iron Man (2008)");
    vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "The Lion King (1994)");
    vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", "Alladin (2019)");
    vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", "The Little Mermaid");
    vdb.AddText("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", "Frozen (2013)");

    // Perform a Vector Search
    var result = vdb.Search(newPrompt, resultsToReturn);

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

## Sample Console App

The [sample console app](src/ConsoleTest/) in this repo show example usage of Build5Nines.SharpVector.dll

It loads a list of movie titles and descriptions from a JSON file, then allows the user to type in prompts to search the database and return the best matches.

Here's a screenshot of the test console app running:

![](assets/build5nines-sharpvector-console-screenshot.jpg)

## Change Log

### v0.9.0-beta

- Add more C# Generics use, so the library is more customizable when used.
- Added `VectorTextResultItem.Similarity` so consuming code can inspect similarity of the Text in the vector search results.
- Update `.Search` method to support search result paging and threshold support for similarity comparison
- Add some basic Unit Tests

### v0.8.0-beta

- Initial release

## Maintained By

The [Build5Nines](https://build5nines.com) SharpVector project is maintained by [Chris Pietschmann](https://pietschsoft.com), Microsoft MVP, HashiCorp Ambassador.
