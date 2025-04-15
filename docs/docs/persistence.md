# Data Persistence

The `Build5Nines.SharpVector` library provides easy-to-use methods for saving a memory-based vector database to a file or stream and loading it again later. This is particularly useful for caching indexed content between runs, deploying pre-built vector stores, or shipping databases with your application.

## File Persistence

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

## Persist to JSON

### Serialize to JSON

To serialize `BasicMemoryVectorDatabase` to a JSON string, use the `SerializeToJson` and `SerializeToJsonAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

// serialize to JSON string
var json = vdb.SerializeToJson();

// serialize to JSON string asynchronously
var json = await vdb.SerializeToJsonAsync();
```

### Deserialize from JSON

To deserialize `BasicMemoryVectorDatabase` from a JSON string, use the `DeserializeFromJson` and `DeserializeFromJsonAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

var json = LoadJson();

// deserialize from JSON string
vdb.DeserializeFromJson(json);

// deserialize from JSON string asynchronously
vdb.DeserializeFromJsonAsync(json);
```

### Write to JSON Stream

To persist your `BasicMemoryVectorDatabase` to a JSON stream, use the `SerializeToJsonStream` or `SerializeToJsonStreamAsync` methods:

```csharp
var vdb = new BasicMemoryVectorDatabase();

var stream = new MemoryStream();

// serialize to JSON stream
vdb.SerializeToJsonStream(stream);

// -- or --

// serialize asynchronously to JSON stream
await vdb.SerializeToJsonStreamAsync(stream);
```

### Read from JSON Stream

To load your `BasicMemoryVectorDatabase` from JSON stream, use the `DeserializeFromJsonStream` and `DeserializeFromJsonStreamAsync` methods:

```csharp
// Be sure Stream position is at the start
stream.Position = 0;

// deserialize from JSON stream
vdb.DeserializeFromJsonStream(stream);

// -- or ---

// deserialize asynchronously from JSON stream
await vdb.DeserializeFromJsonStreamAsync(stream);
```
