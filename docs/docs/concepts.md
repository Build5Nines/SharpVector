# Concepts

## What is a Vector Database?

A vector database is a type of database optimized for storing and querying data represented as high-dimensional vectors. Instead of traditional structured queries based on exact values (like SQL), vector databases allow you to perform similarity searches — finding items that are "close" to a given vector in terms of meaning or context.

This is particularly useful for:

- Semantic Search – retrieving documents or text that are contextually similar, even if the keywords don't match

- Recommendations – suggesting similar content based on user interaction

- AI Applications – powering chatbots, embeddings, and classification models with context-aware search

SharpVector brings this powerful concept to the .NET ecosystem in a lightweight, in-memory format. You can use it to build applications that need fast, context-aware search and ranking over text data — without needing a heavy external vector database engine.

In a vector database:

- Text is converted into embeddings (arrays of numbers) using models like OpenAI, HuggingFace, or your own

- Those vectors are stored in the database

- Queries are also vectors, and the database finds items most similar to that vector using metrics like cosine similarity

> With SharpVector, you get control over vector generation and comparison - all within your .NET application.

## Text Vectorization

Text vectorization is the process of transforming human-readable text into a numerical format — specifically, a fixed-size array of floating point numbers called a vector or embedding. This enables computers to measure the semantic similarity between different pieces of text.

Vector embeddings capture the meaning of the text, not just the exact words. For example:

- `The cat sat on the mat`

- `A feline rested on the rug`

Both phrases have similar meanings, and their embeddings would be close in vector space — even though they don’t share any exact words.

### How Vectorization Works

By default, `Build5Nines.SharpVector` include functionality to automatically generate vectors locally within the library without the need for any text embedding model. Alternatively, a text embedding model (such as OpenAI’s `text-embedding-ada-002` or a local transformer from HuggingFace) could also be used to take a string of text and generate the vectors.

The vectors are generated often hundreds of dimensions long — that captures the semantic content.

These vector are then indexed and compared to others using mathematical similarity functions to search for and find matches for text queries.

### Local Vectorization

SharpVector is embedding-agnostic — it supports the following methods of generating text vectors:

- Automatically generate vectors locally, no embeddings model required

- Connect to remote OpenAI or Azure OpenAI embeddings models if required

This design gives you flexibility and decouples SharpVector from any specific ML model or service.

<!-- ## How SharpVector Stores Data

Structure of a vector record: vector + text + optional metadata

VectorDatabase<T> class and its generic metadata support

Memory-only storage design (no persistence layer)

Efficiency and limits of in-memory approach

## Similarity Search

How similarity is calculated

Default algorithm: cosine similarity

Other supported measures: Euclidean, Manhattan (if applicable)

Custom similarity function support

## Metadata & Tagging

Associating custom metadata with each vector

Filtering/searching based on metadata

Practical examples: source IDs, tags, timestamps

## Search Strategies

topK search and score explanation

Multi-vector or bulk search patterns

Use cases for ranked results

## Extensibility

Custom vectorizer pipeline (external embedding model)

Plug-and-play similarity functions

Extending metadata filtering capabilities

## Best Practices

Embedding consistency (keep vector dimensions aligned)

Normalizing vectors

Managing memory usage

Tips for batch processing -->

## Retrieval Augmented Generation (RAG)

A vector database plays a central role in [Retrieval Augmented Generation (RAG)](https://build5nines.com/what-is-retrieval-augmented-generation-rag/) — a technique where search is combined with generative AI models (like GPT or other LLMs) to produce more accurate, context-aware responses.

In a typical RAG workflow:

- A user's query is vectorized and used to search a vector database.

- The most semantically relevant chunks of text are retrieved.

- These retrieved results are then fed into a generative model as additional context.

This process allows the LLM to generate responses grounded in real, domain-specific information that it may not have seen during training. It enhances factual accuracy and relevance — making AI-driven experiences more trustworthy and useful.

`Build5Nines.SharpVector` makes it easy to build RAG-based workflows in .NET by enabling fast, in-memory vector search of documents, notes, or knowledge bases. Combined with an embedding service like OpenAI or Azure OpenAI, it’s a powerful foundation for building intelligent AI apps.
