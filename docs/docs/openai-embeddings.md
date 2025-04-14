# OpenAI Embeddings

Integrating OpenAI embeddings with **Build5Nines.SharpVector** enhances the semantic search capabilities of your .NET applications. By leveraging models like `text-embedding-ada-002`, you can generate high-quality vector representations of text, leading to more accurate and contextually relevant search results.

## Why Use OpenAI Embeddings?

While **SharpVector** includes basic embedding generation, utilizing OpenAI's advanced models offers significant advantages:

- **Improved Search Accuracy**: OpenAI's embeddings capture the semantic meaning of text, resulting in more relevant search outcomes.
- **Pre-trained on Extensive Data**: These models are trained on vast datasets, enhancing their robustness and generalization capabilities.
- **Optimized for Performance**: Designed for efficient retrieval and indexing, OpenAI embeddings facilitate faster search operations.

## Getting Started

### Installation

To integrate OpenAI embeddings with SharpVector, install the `Build5Nines.SharpVector.OpenAI` NuGet package:

=== ".NET CLI"
    ```bash
    dotnet add package Build5Nines.SharpVector.OpenAI
    ```
=== "Package Manager"
    ```powershell
    Nuget\Install-Package Build5Nines.SharpVector.OpenAI
    ```

This package includes the core `Build5Nines.SharpVector` library and dependencies required to connect to OpenAI's embedding services.

### Setting Up the Embedding Client

=== "OpenAI"
    If you're using OpenAI's API directly:

    ```csharp
    using OpenAI;
    
    var openAIKey = "your-api-key";
    var modelName = "text-embedding-ada-002";
    
    var openAIClient = new OpenAIClient(openAIKey);
    var embeddingClient = openAIClient.GetEmbeddingClient(modelName);
    ```
=== "Azure OpenAI"
    For applications utilizing Azure OpenAI:

    ```csharp
    using Azure;
    using Azure.AI.OpenAI;
    
    var openAIUri = new Uri("https://your-resource-name.openai.azure.com/");
    var openAIKey = "your-api-key";
    var modelName = "text-embedding-ada-002";
    
    var openAIClient = new AzureOpenAIClient(openAIUri, new AzureKeyCredential(openAIKey));
    var embeddingClient = openAIClient.GetEmbeddingClient(modelName);
    ```

### Initializing the Vector Database

With the embedding client set up, initialize the in-memory vector database:

```csharp
using Build5Nines.SharpVector.OpenAI;

var vectorDatabase = new BasicOpenAIMemoryVectorDatabase(embeddingClient);
```

- `embeddingClient`: The OpenAI Embedding Client ot use for generating the vector embeddings.

## Adding Text Data

To add text documents to the vector database:

```csharp
// sync
vectorDatabase.AddText(documentText, metadataText);

// async
await vectorDatabase.AddTextAsync(documentText, metadataText);
```

- `documentText`: The textual content to be vectorized.
- `metadataText`: Associated metadata (e.g., document title, JSON string) stored alongside the vectorized text.

!!! note
    Metadata is not vectorized but is retrieved with search results, providing context.

## Performing Similarity Search

The `SearchAsync` method returns documents whose vector representations closely match the query vector, based on similarity metrics like cosine similarity.

```csharp
var query = "your search query";
var results = await vectorDatabase.SearchAsync(query);
```

The `.SearchAsync` method supports additional arguments to help with searching the vector database:

```csharp
var results = await vectorDatabase.SearchAsync(queryText,
  threshold: 0.001f // 0.2f - Cosine Similarity
  pageIndex: 0, // page index of search results (default: 0)
  pageCount: 10 // Number of results per page to return (default: no limit)
);
```

- `queryText`: The text query to search within the vector database.
- `threshold`: The similarity threshold to use for searching the vector database using Cosine Similarity method.
- `pageIndex`: The page index of search results to return. Default is `0`.
- `pageCount`: The number of search results to return per page. Default is "no limit" (aka return all results)

## Summary

Integrating OpenAI embeddings with **Build5Nines.SharpVector** empowers your .NET applications with advanced semantic search capabilities. By leveraging high-quality vector representations, you can achieve more accurate and context-aware search results, enhancing the overall user experience.
