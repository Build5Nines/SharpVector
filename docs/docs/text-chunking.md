# Text Chunking

**Text chunking** is the process of breaking up large documents into smaller segments ("chunks") before embedding and storing them in a vector database. This allows for more accurate semantic search and improves performance in applications that deal with large bodies of text.

SharpVector supports several chunking strategies via the `TextDataLoader` class, making it easy to break down documents automatically.

---

## Why Chunk Text?

Chunking large documents improves search quality by:

- Isolating meaningful sections for embedding (e.g. a paragraph or sentence)
- Reducing noise and improving semantic match precision
- Allowing matches on specific pieces of content rather than full documents

---

## Getting Started with `TextDataLoader`

### Add References

Add the necessary namespaces for the `TextDataLoader` class:

```csharp
using Build5Nines.SharpVector.Data;
```

### Instantiate Vector Database

Create a new vector database. This is the vector database you will be loading chunked text into.

```csharp
using Build5Nines.SharpVector;

var vdb = new BasicMemoryVectorDatabase();
```

To use the `TextDataLoader`, you need to instantiate an instance of the class passing in the necessary types that match the `TId` and `TMetadata` of the `IVectorDatabase<>` interface. The `BasicMemoryVectorDatabase` class is setup with these types:

- `TId` of `int`: This is the type for the internal ID for Text items in the vector database.
- `TMetadata` of `string`: This is the type of the Metadata object stored along with the Text items in the vector database.

!!! info
    Most cases you'll likely be using the `BasicMemoryVectorDatabase` class, but the library incudes interfaces and base classes to allow for extensibility to use different `TId` and `TMetadata` types as necessary.

### Instantiate `TextDataLoader`

```csharp
var loader = new TextDataLoader<int, string>(vdb);
```

---

## Chunking Methods

The `TextDataLoader<TKey, TValue>` class can be used to load documents into the vector database with automatic chunking and metadata assignment. Each chunk must be associated with some metadata — even if it's just a minimal description — using the `RetrieveMetadata` function.

=== "Paragraph"

    Splits the text into logical paragraphs:
    
    ```csharp
    string document = LoadDocumentText();
    loader.AddDocument(document, new TextChunkingOptions<string>
    {
        Method = TextChunkingMethod.Paragraph,
        RetrieveMetadata = (chunk) => {
            return "{ \"chunkSize\": \"" + chunk.Length + "\" }";
        }
    });
    ```

=== "Sentence"

    Breaks text into individual sentences using punctuation boundaries:
    
    ```csharp
    string document = LoadDocumentText();
    loader.AddDocument(document, new TextChunkingOptions<string>
    {
        Method = TextChunkingMethod.Sentence,
        RetrieveMetadata = (chunk) => {
            return "{ \"chunkSize\": \"" + chunk.Length + "\" }";
        }
    });
    ```

=== "Fixed-Length"

    Divides the text into fixed character lengths, useful for very large documents or uniform sections:
    
    ```csharp
    string document = LoadDocumentText();
    loader.AddDocument(document, new TextChunkingOptions<string>
    {
        Method = TextChunkingMethod.FixedLength,
        ChunkSize = 150,
        RetrieveMetadata = (chunk) => {
            return "{ \"chunkSize\": \"" + chunk.Length + "\" }";
        }
    });
    ```

=== "Overlapping Window"

    Split the text into overlapping windows.

    ```csharp
    string document = LoadDocumentText();
    loader.AddDocument(document, new TextChunkingOptions<string>
    {
        Method = TextChunkingMethod.OverlappingWindow,
        ChunkSize = 150,
        // Number of words to overlap text chunks
        OverlapSize = 50,
        RetrieveMetadata = (chunk) => {
            return "{ \"chunkSize\": \"" + chunk.Length + "\" }";
        }
    }
    ```

> 🧠 Tip: Use chunking method and size that best aligns with your content type and retrieval goals.

---

## Customize Metadata

The `RetrieveMetadata` delegate allows you to generate metadata per chunk. The following example will store a JSON string as the metadata that contains the filename of the document and the date/time it was indexed into the vector database.

```csharp
string filename = "document.txt";
string document = LoadDocumentText(filename);
loader.AddDocument(document, new TextChunkingOptions<string>
{
    Method = TextChunkingMethod.Paragraph,
    RetrieveMetadata = (chunk) => {
        var json = JsonSerializer.Serialize(new {
            documentFileName = filename,
            timeIndexed = DataTime.UtcNow.ToString("o")
        });
        return json;
    }
});
```

This metadata is stored alongside each vector and returned in search results, allowing context-aware interfaces.

---

## Summary

Chunking text before indexing enhances SharpVector's ability to deliver relevant and focused semantic search results. With support for multiple chunking strategies and flexible metadata, it's easy to adapt to different content and application needs.

Use `TextDataLoader` to simplify loading, chunking, and organizing your text data — and supercharge your vector search accuracy!
