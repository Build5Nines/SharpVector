---
title: Discover SharpVector
description: A lightweight, in-memory text vector database for .NET that powers intelligent search and recommendation features.
---
# Build5Nines.SharpVector

**SharpVector** is a lightweight, in-memory text vector database built for .NET applications. It enables fast and flexible vector-based similarity search for text data — ideal for search engines, recommendation systems, semantic analysis, and AI-enhanced features.

![Tests: Passing](https://img.shields.io/github/actions/workflow/status/build5nines/sharpvector/dotnet-tests.yml?label=tests)
![Build: Passing](https://img.shields.io/github/actions/workflow/status/build5nines/sharpvector/build-release.yml)
![Libraries.io dependency status for GitHub repo](https://img.shields.io/librariesio/github/build5nines/sharpvector)

[![NuGet](https://img.shields.io/nuget/v/Build5Nines.SharpVector.svg)](https://www.nuget.org/packages/Build5Nines.SharpVector/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![Framework: .NET 8+](https://img.shields.io/badge/framework-.NET%208%2B-blue)
![Semantic Search: Enabled](https://img.shields.io/badge/semantic%20search-enabled-purple)
![Gen AI: Ready](https://img.shields.io/badge/Gen%20AI-ready-purple)

<!--
![GitHub contributors](https://img.shields.io/github/contributors/Build5Nines/SharpVector)
<!-- https://buttons.github.io/ -->
<!--
<a class="github-button" href="https://github.com/Build5Nines/SharpVector" data-color-scheme="no-preference: light; light: light; dark: dark;" data-icon="octicon-star" data-size="large" data-show-count="true" aria-label="Star Build5Nines/SharpVector on GitHub">Star</a>
-->

Vector databases are used with [Generative AI](https://build5nines.com/what-is-generative-ai/?utm_source=github&utm_medium=sharpvector) solutions augmenting the LLM (Large Language Model) with the ability to load additional context data with the AI prompt using the [RAG (Retrieval-Augmented Generation)](https://build5nines.com/what-is-retrieval-augmented-generation-rag/?utm_source=github&utm_medium=sharpvector) design pattern.

While there are lots of large databases that can be used to build Vector Databases (like Azure CosmosDB, PostgreSQL w/ pgvector, Azure AI Search, Elasticsearch, and more), there are not many options for a lightweight vector database that can be embedded into any .NET application. Build5Nines SharpVector is the lightweight in-memory Text Vector Database for use in any .NET application that you're looking for!

---

> "For the in-memory vector database, we're using Build5Nines.SharpVector, an excellent open-source project by Chris Pietschmann. SharpVector makes it easy to store and retrieve vectorized data, making it an ideal choice for our sample RAG implementation."
>
> _- [Tulika Chaudharie, Principal Product Manager at Microsoft for Azure App Service](https://azure.github.io/AppService/2024/09/03/Phi3-vector.html)_

---

## 🚀 Key Features

- 🔍 **Text Embedding & Search** – Store and search documents using vector similarity (cosine, Euclidean, etc.)
- 🧠 **Pluggable Embeddings** – Works with your own vectorizer (like OpenAI, HuggingFace, or custom)
- ⚡ **In-Memory Performance** – Designed for speed in local or embedded applications
- 🛠️ **Lightweight & Easy to Use** – Minimal dependencies, simple API
- 📦 **Custom Metadata** - Store additional metadata with each text entry stored in the vector database.
- 🛠️ **Supports async/await** - Async methods for scalable and non-blocking database operations.
- ⚡ **Vector Comparisons** - Supports various vector comparison methods for searching similar texts. Including cosine similarity (by default), and configurable for Euclidean distance. Or write your own custom vector comparison algorithm.

---

## 🧠 Use Cases

An in-memory vector databases like `Build5Nines.SharpVector` provides several advantages over a traditional vector database server, particularly in scenarios that might demand high performance, low latency, and efficient resource usage.

SharpVector is great for:

- Embedding search within desktop or server .NET applications

- Building semantic search over documents or notes

- Powering recommendation features based on text similarity

- Integrating with OpenAI or local embeddings for intelligent querying

- Offline or Edge computing with limited or no internet connectivity

- Development and testing without the overhead of installing a server

---

## 🙌 Get Involved

We welcome contributions, feedback, and new ideas. Whether it's a bug report or a pull request, head over to our GitHub repository to start collaborating!

---

The **Build5Nines SharpVector** project is maintained by [Chris Pietschmann](https://pietschsoft.com?utm_source=github&utm_medium=sharpvector), founder of [Build5Nines](https://build5nines.com?utm_source=github&utm_medium=sharpvector), Microsoft MVP, HashiCorp Ambassador, and Microsoft Certified Trainer (MCT).
