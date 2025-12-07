---
title: Data Persistence
---
# :octicons-file-24: Data Persistence

The `Build5Nines.SharpVector` library provides easy-to-use methods for saving a memory-based vector database to a file or stream and loading it again later. This is particularly useful for caching indexed content between runs, deploying pre-built vector stores, or shipping databases with your application.

---

## :material-file: File Persistence

`Build5Nines.SharpVector` supports persisting the vector database to a file.

!!! info
    This functionality is implemented as methods available to both the `Build5Nines.SharpVector.BasicMemoryVectorDatabase` and `Build5Nines.SharpVector.OpenAI.BasicOpenAIMemoryVectorDatabase`. These methods are actually extensions on the base `IVectorDatabase` interface, so all implementations of this interface will have this capability.

### Save to File

To persist your `BasicMemoryVectorDatabase` to disk, use the `SaveToFile` or `SaveToFileAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

var filePath = "vectordata.b59vdb";

// persist vector database to file asynchronously
await vdb.SaveToFileAsync(filePath);

// -- or --

// persist vector database to file
vdb.SaveToFile(filePath);
```

!!! info
    The file extension used in this example is `.b59vdb`, however this is arbitrary. The library doesn't look at the file extension. It only reads the binary contents of the file; which is actually in ZIP file format.

### Load from File

To load a previously saved vector database from disk, use the `LoadFromFile` or `LoadFromFileAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

var filePath = "vectordata.b59vdb";

// load vector database from file
vdb.LoadFromFile(filePath);

// -- or --

// load vector database from file asynchronously
await vdb.LoadFromFileAsync(filePath);
```

---

## :material-file-move: Persist to Stream

The underlying methods used by `SaveToFile` and `LoadFromFile` methods for serializing the vector database to a `Stream` are available to use directly. This provides support for reading/writing to `MemoryStream` (or other streams) if the vector database needs to be persisted to something other than the local file system.

!!! info
    These `SerializeToBinaryStream` and `DeserializeFromBinaryStream` methods are available in `v2.0.2` and later.

### Write to Stream

To persist your `BasicMemoryVectorDatabase` to a JSON stream, use the `SerializeToBinaryStream` or `SerializeToBinaryStreamAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

var stream = new MemoryStream();

// serialize to JSON stream
vdb.SerializeToBinaryStream(stream);

// -- or --

// serialize asynchronously to JSON stream
await vdb.SerializeToBinaryStreamAsync(stream);
```

### Read from Stream

To load your `BasicMemoryVectorDatabase` from JSON stream, use the `DeserializeFromBinaryStream` and `DeserializeFromBinaryStreamAsync` methods:

```csharp
// Be sure Stream position is at the start
stream.Position = 0;

// deserialize from JSON stream
vdb.DeserializeFromBinaryStream(stream);

// -- or ---

// deserialize asynchronously from JSON stream
await vdb.DeserializeFromBinaryStreamAsync(stream);
```

---

## :material-database-outline: BasicDiskVectorDatabase

The `BasicDiskVectorDatabase` provides a basic vector database implementation that automatically stores the vector store and vocabulary store to disk. It's implmentation of vectorization is the same as the `BasicMemoryVectorDatabase`, but with the modification that it automatically persists the database to disk in the background to the specified folder path.

Here's a basic example of using `BasicDiskVectorDatabase`:

```csharp
// specify the folder where to persist the database data on disk
var vdb = new BasicDiskVectorDatabase("C:/data/content-db");
foreach (var doc in documents)
{
    vdb.AddText(doc.Id, doc.Text);
}

var results = vdb.Search("some text");

```

### Tips

- Prefer absolute paths for the storage folder in production services.
- Place the folder on fast storage (SSD) for best indexing/query performance.
- Avoid sharing the same folder across multiple processes concurrently.
- Back up the folder regularly to preserve your vector store and vocabulary.
