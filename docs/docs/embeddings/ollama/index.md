---
title: Ollama Embeddings
icon: simple-ollama
description: Integrate Ollama embedding models with SharpVector to supercharge your semantic search and AI features in .NET apps.
---

# :simple-ollama: Ollama Embeddings

Integrating [Ollama](https://ollama.com) embedding modes with `Build5Nines.SharpVector` enhances the semantic search capabilities of your .NET applications. By leveraging models like `nomic-embed-text` or others, you can generate higher quality vector representations of text, leading to more accurate and contextually relevant search results.

## Why Use an Ollama Embedding Model?

While **SharpVector** includes basic embedding generation, utilizing an Ollama embedding model offers significant advantages:

- **Improved Search Accuracy**: Embedding models capture the semantic meaning of text more accurately, resulting in more relevant search outcomes.
- **Pre-trained on Extensive Data**: These models are trained on vast datasets, enhancing their robustness and generalization capabilities.
- **Optimized for Performance**: Designed for efficient retrieval and indexing, Ollama embedding models facilitate faster search operations.

## Getting Started

To integrate an Ollama embedding model with SharpVector, install the `Build5Nines.SharpVector.Ollama` NuGet package:

=== ".NET CLI"
    ```bash
    dotnet add package Build5Nines.SharpVector.Ollama
    ```
=== "Package Manager"
    ```powershell
    Nuget\Install-Package Build5Nines.SharpVector.Ollama
    ```

This package includes the core `Build5Nines.SharpVector` library and dependencies required to connect to Ollama's embedding API.

## Initialize the Vector Database using Ollama

With the Ollama embedding model running, initialize the the **SharpVector** database:

```csharp
using Build5Nines.SharpVector.Ollama;

var modelName = "nomic-embed-text";

// For connecting to Locally running ('localhost') Ollama
var vectorDatabase = new BasicOllamaMemoryVectorDatabase(modelName)

// For connecting to a different Ollama endpoint URL
var ollamaEndpoint = "http:/localhost:11434/api/embeddings";
var vactorDatabase = new BasicOllamaMemoryVectorDatabase(ollamaEndpoint, modelName);
```

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

Integrating an Ollama embedding model with **Build5Nines.SharpVector** empowers your .NET applications with advanced semantic search capabilities. By leveraging high-quality vector representations, you can achieve more accurate and context-aware search results, enhancing the overall user experience.
