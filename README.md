# Build5Nines SharpVector - The lightweight, in-memory, local, Semantic Search, Text Vector Database for any C# / .NET Applications

`Build5Nines.SharpVector` is an in-memory vector database library designed for .NET applications. It allows you to store, search, and manage text data using vector representations. The library is customizable and extensible, enabling support for different vector comparison methods, preprocessing techniques, and vectorization strategies.

[![.NET Core Tests](https://github.com/Build5Nines/SharpVector/actions/workflows/dotnet-tests.yml/badge.svg)](https://github.com/Build5Nines/SharpVector/actions/workflows/dotnet-tests.yml)
[![Build and Release](https://github.com/Build5Nines/SharpVector/actions/workflows/build-release.yml/badge.svg)](https://github.com/Build5Nines/SharpVector/actions/workflows/build-release.yml)
![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/build5nines/sharpvector)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![Framework: .NET 8+](https://img.shields.io/badge/framework-.NET%208%2B-blue)
![Semantic Search: Enabled](https://img.shields.io/badge/semantic%20search-enabled-purple)
![Gen AI: Ready](https://img.shields.io/badge/Gen%20AI-ready-purple)

Vector databases are used with Semantic Search and [Generative AI](https://build5nines.com/what-is-generative-ai/?utm_source=github&utm_medium=sharpvector) solutions augmenting the LLM (Large Language Model) with the ability to load additional context data with the AI prompt using the [RAG (Retrieval-Augmented Generation)](https://build5nines.com/what-is-retrieval-augmented-generation-rag/?utm_source=github&utm_medium=sharpvector) design pattern.

While there are lots of large databases that can be used to build Vector Databases (like Azure CosmosDB, PostgreSQL w/ pgvector, Azure AI Search, Elasticsearch, and more), there are not many options for a lightweight vector database that can be embedded into any .NET application to provide a local text vector database.

> "For the in-memory vector database, we're using Build5Nines.SharpVector, an excellent open-source project by Chris Pietschmann. SharpVector makes it easy to store and retrieve vectorized data, making it an ideal choice for our sample RAG implementation."
>
> [Tulika Chaudharie, Principal Product Manager at Microsoft for Azure App Service](https://azure.github.io/AppService/2024/09/03/Phi3-vector.html)

Build5Nines SharpVector is the lightweight, local, in-memory Text Vector Database for implementing semantic search into any .NET application!

### [Documentation](https://sharpvector.build5nines.com) | [Get Started](https://sharpvector.build5nines.com/get-started/) | [Samples](https://sharpvector.build5nines.com/samples/)

## Nuget Package

The `Build5Nines.SharpVector` library is available as a Nuget Package to easily include into your .NET projects:

```bash
dotnet add package Build5Nines.SharpVector
```

You can view it on Nuget.org here: <https://www.nuget.org/packages/Build5Nines.SharpVector/>

## Maintained By

The **Build5Nines SharpVector** project is maintained by [Chris Pietschmann](https://pietschsoft.com?utm_source=github&utm_medium=sharpvector), founder of [Build5Nines](https://build5nines.com?utm_source=github&utm_medium=sharpvector), Microsoft MVP, HashiCorp Ambassador, and Microsoft Certified Trainer (MCT).
