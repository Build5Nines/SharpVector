---
title: Get Started
description: Get up and running with SharpVector in minutes. Learn how to install, initialize, and begin storing and searching vectorized text data.
---
# Get Started

It's really easy to get started with using `Build5Nines.SharpVector`. Simply follow the below steps.

## Prerequisites

Using `Build5Nines.SharpVector` requires the following:

- .NET 8.0 or later

## Install Nuget Package

The `Build5Nines.SharpVector` library is available as a [Nuget package](https://www.nuget.org/packages/Build5Nines.SharpVector):

=== ".NET CLI"
    ```bash
    dotnet add package Build5Nines.SharpVector
    ```

=== "Package Manager"
    ```powershell
    Nuget\Install-Package Build5Nines.SharpVector
    ```

## Basic example

The following is a basic example of using `Build5Nines.SharpVector` to create and use an in-memory vector database within a C# application:

```csharp
using Build5Nines.SharpVector;

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

if (!result.IsEmpty)
{
    Console.WriteLine("Similar Text Found:");
    foreach (var item in result.Texts)
    {
        Console.WriteLine(item.Metadata);
        Console.WriteLine(item.Text);
    }
} else {
    Console.WriteLine("No results found.");
}
```

🚀 You are now using an in-memory vector database to implement semantic text searching within your app!

!!! info
    The `Build5Nines.SharpVector` library, be default, supports local text vector generation. However, there is also additional support for both [OpenAI and Ollama embeddings models](../embeddings) for using higher quality, more robust vector generation.
