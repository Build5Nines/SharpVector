---
title: Protocol Buffers Serialization
---
# :octicons-file-binary-24: Protocol Buffers Serialization

Protocol Buffers (protobuf) is a language-neutral, platform-neutral extensible mechanism for serializing structured data developed by Google. While SharpVector natively uses a JSON-based binary format wrapped in a ZIP archive, you can use Protocol Buffers for serialization with SharpVector databases.

---

## :material-help-circle: Feasibility Assessment

**YES, it is possible** to serialize SharpVector databases using Protocol Buffers! 

The SharpVector library provides serialization methods (`SerializeToBinaryStream` and `DeserializeFromBinaryStream`) that work with streams. This means you can:

1. Serialize the SharpVector database to a stream using the native method
2. Convert the stream data to a Protocol Buffers message
3. Use Protocol Buffers for network transmission or storage
4. Deserialize the Protocol Buffers message back to a stream
5. Load the stream back into SharpVector

Alternatively, you can create Protocol Buffers definitions that mirror SharpVector's data structures and convert between them.

---

## :material-package-variant: Approach 1: Wrapping Native Serialization

This approach wraps SharpVector's native binary serialization in a Protocol Buffers message. This is the simplest approach and maintains full compatibility with SharpVector's serialization format.

### Step 1: Install Required Packages

First, install the required NuGet packages:

```bash
dotnet add package Build5Nines.SharpVector
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
```

### Step 2: Define Protocol Buffers Schema

Create a `.proto` file (e.g., `vectordb.proto`):

```protobuf
syntax = "proto3";

package sharpvector;

// Wrapper message for SharpVector database binary data
message VectorDatabaseWrapper {
  // The binary serialized SharpVector database (in ZIP format)
  bytes database_data = 1;
  
  // Metadata about the database (optional)
  string database_type = 2;
  string version = 3;
  int64 timestamp = 4;
}
```

### Step 3: Configure Project File

Update your `.csproj` file to include the Protocol Buffers compiler:

```xml
<ItemGroup>
  <PackageReference Include="Google.Protobuf" Version="3.25.1" />
  <PackageReference Include="Grpc.Tools" Version="2.60.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>

<ItemGroup>
  <Protobuf Include="vectordb.proto" GrpcServices="None" />
</ItemGroup>
```

### Step 4: Implement Serialization

```csharp
using Build5Nines.SharpVector;
using Google.Protobuf;
using Sharpvector; // Generated from vectordb.proto

public class ProtobufVectorDatabaseSerializer
{
    /// <summary>
    /// Serializes a SharpVector database to Protocol Buffers format
    /// </summary>
    public static byte[] SerializeToProtobuf<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        string databaseType = null)
        where TId : notnull
    {
        // First, serialize the database to SharpVector's native binary format
        using var memoryStream = new MemoryStream();
        database.SerializeToBinaryStream(memoryStream);
        
        // Get the binary data
        var databaseData = memoryStream.ToArray();
        
        // Create the Protocol Buffers wrapper
        var wrapper = new VectorDatabaseWrapper
        {
            DatabaseData = ByteString.CopyFrom(databaseData),
            DatabaseType = databaseType ?? database.GetType().FullName,
            Version = "1.0",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        // Serialize to Protocol Buffers
        return wrapper.ToByteArray();
    }
    
    /// <summary>
    /// Deserializes a SharpVector database from Protocol Buffers format
    /// </summary>
    public static void DeserializeFromProtobuf<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        byte[] protobufData)
        where TId : notnull
    {
        // Deserialize the Protocol Buffers wrapper
        var wrapper = VectorDatabaseWrapper.Parser.ParseFrom(protobufData);
        
        // Extract the binary database data
        var databaseData = wrapper.DatabaseData.ToByteArray();
        
        // Deserialize into SharpVector database
        using var memoryStream = new MemoryStream(databaseData);
        database.DeserializeFromBinaryStream(memoryStream);
    }
    
    /// <summary>
    /// Async version: Serializes a SharpVector database to Protocol Buffers format
    /// </summary>
    public static async Task<byte[]> SerializeToProtobufAsync<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        string databaseType = null)
        where TId : notnull
    {
        using var memoryStream = new MemoryStream();
        await database.SerializeToBinaryStreamAsync(memoryStream);
        
        var databaseData = memoryStream.ToArray();
        
        var wrapper = new VectorDatabaseWrapper
        {
            DatabaseData = ByteString.CopyFrom(databaseData),
            DatabaseType = databaseType ?? database.GetType().FullName,
            Version = "1.0",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        return wrapper.ToByteArray();
    }
    
    /// <summary>
    /// Async version: Deserializes a SharpVector database from Protocol Buffers format
    /// </summary>
    public static async Task DeserializeFromProtobufAsync<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        byte[] protobufData)
        where TId : notnull
    {
        var wrapper = VectorDatabaseWrapper.Parser.ParseFrom(protobufData);
        var databaseData = wrapper.DatabaseData.ToByteArray();
        
        using var memoryStream = new MemoryStream(databaseData);
        await database.DeserializeFromBinaryStreamAsync(memoryStream);
    }
}
```

### Step 5: Usage Example

```csharp
using Build5Nines.SharpVector;

// Create and populate a vector database
var database = new BasicMemoryVectorDatabase();
database.AddText("Hello world");
database.AddText("Protocol Buffers serialization");
database.AddText("Vector database persistence");

// Serialize to Protocol Buffers
var protobufData = ProtobufVectorDatabaseSerializer.SerializeToProtobuf(database);

// Save to file
File.WriteAllBytes("database.pb", protobufData);

// Later: Load from Protocol Buffers
var loadedDatabase = new BasicMemoryVectorDatabase();
var loadedProtobufData = File.ReadAllBytes("database.pb");
ProtobufVectorDatabaseSerializer.DeserializeFromProtobuf(loadedDatabase, loadedProtobufData);

// Verify the data was loaded
var results = loadedDatabase.Search("serialization");
Console.WriteLine($"Found {results.TotalCount} results");
```

---

## :material-database-export: Approach 2: Native Protocol Buffers Schema

This approach creates Protocol Buffers definitions that directly mirror SharpVector's internal data structures. This provides more control and interoperability but requires more implementation effort.

### Step 1: Define Complete Schema

Create a more detailed `.proto` file (e.g., `vectordb_native.proto`):

```protobuf
syntax = "proto3";

package sharpvector.native;

// A single vector text item with metadata
message VectorTextItem {
  string text = 1;
  repeated float vector = 2;
  string metadata_json = 3;  // Serialized metadata as JSON
}

// A complete vector database
message VectorDatabase {
  map<string, VectorTextItem> items = 1;
  map<string, int32> vocabulary = 2;
  string database_version = 3;
  int64 created_timestamp = 4;
  int64 updated_timestamp = 5;
}
```

### Step 2: Implement Converters

```csharp
using Build5Nines.SharpVector;
using Google.Protobuf;
using Sharpvector.Native; // Generated from vectordb_native.proto
using System.Text.Json;

public class NativeProtobufConverter
{
    /// <summary>
    /// Converts a SharpVector database to native Protocol Buffers format
    /// </summary>
    public static VectorDatabase ToProtobuf(BasicMemoryVectorDatabase database)
    {
        var protobufDb = new VectorDatabase
        {
            DatabaseVersion = "1.0",
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            UpdatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        // Convert each item in the database
        foreach (var item in database)
        {
            var vectorTextItem = new VectorTextItem
            {
                Text = item.Text.ToString(),
                MetadataJson = JsonSerializer.Serialize(item.Metadata)
            };
            
            // Add vector values
            vectorTextItem.Vector.AddRange(item.Vector);
            
            // Add to the map using the ID as key
            protobufDb.Items.Add(item.Id.ToString(), vectorTextItem);
        }
        
        return protobufDb;
    }
    
    /// <summary>
    /// Converts from Protocol Buffers format to SharpVector database
    /// </summary>
    public static void FromProtobuf(BasicMemoryVectorDatabase database, VectorDatabase protobufDb)
    {
        foreach (var kvp in protobufDb.Items)
        {
            var metadata = string.IsNullOrEmpty(kvp.Value.MetadataJson) 
                ? null 
                : JsonSerializer.Deserialize<string>(kvp.Value.MetadataJson);
            
            database.AddText(kvp.Value.Text, metadata);
        }
    }
}
```

### Step 3: Usage Example

```csharp
var database = new BasicMemoryVectorDatabase();
database.AddText("Sample text 1");
database.AddText("Sample text 2");

// Convert to native Protocol Buffers format
var protobufDb = NativeProtobufConverter.ToProtobuf(database);

// Serialize to bytes
var bytes = protobufDb.ToByteArray();

// Save or transmit the bytes
File.WriteAllBytes("database_native.pb", bytes);

// Later: Deserialize
var loadedProtobufDb = VectorDatabase.Parser.ParseFrom(File.ReadAllBytes("database_native.pb"));
var newDatabase = new BasicMemoryVectorDatabase();
NativeProtobufConverter.FromProtobuf(newDatabase, loadedProtobufDb);
```

---

## :material-scale-balance: Comparison of Approaches

| Aspect | Approach 1 (Wrapper) | Approach 2 (Native) |
|--------|---------------------|---------------------|
| **Complexity** | Simple | Moderate |
| **Compatibility** | Perfect - uses native format | Requires conversion logic |
| **Size** | Slightly larger (includes ZIP overhead) | Potentially smaller |
| **Performance** | Fast (minimal conversion) | Slower (requires conversion) |
| **Interoperability** | Limited to SharpVector | Better for cross-platform |
| **Maintenance** | Easier - follows SharpVector updates | Requires updates when SharpVector changes |

---

## :material-network: Use Cases for Protocol Buffers

Protocol Buffers serialization is particularly useful for:

### 1. **Microservices Communication**
```csharp
// Service A: Serialize and send via gRPC
var protobufData = await ProtobufVectorDatabaseSerializer.SerializeToProtobufAsync(database);
await grpcClient.SendDatabaseAsync(new DatabaseRequest { Data = ByteString.CopyFrom(protobufData) });

// Service B: Receive and deserialize
var receivedData = response.Data.ToByteArray();
await ProtobufVectorDatabaseSerializer.DeserializeFromProtobufAsync(newDatabase, receivedData);
```

### 2. **Cloud Storage with Metadata**
```csharp
// Upload to cloud storage
var wrapper = new VectorDatabaseWrapper
{
    DatabaseData = ByteString.CopyFrom(serializedData),
    DatabaseType = "BasicMemoryVectorDatabase",
    Version = "2.2.0",
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
};
await cloudStorage.UploadAsync("vectordb.pb", wrapper.ToByteArray());
```

### 3. **Cross-Language Integration**
Protocol Buffers allows you to work with SharpVector databases in other languages (Python, Java, Go, etc.) by deserializing the wrapper and processing the binary data.

---

## :material-star: Recommendations

- **Use Approach 1 (Wrapper)** if you:
  - Want the simplest implementation
  - Need full compatibility with SharpVector's format
  - Plan to use Protocol Buffers primarily for transport/storage

- **Use Approach 2 (Native)** if you:
  - Need cross-language/cross-platform interoperability
  - Want to process the data in non-.NET environments
  - Need fine-grained control over the serialization format

- **Use SharpVector's Native Serialization** if you:
  - Only need .NET-to-.NET communication
  - Don't require Protocol Buffers' specific benefits
  - Want the best performance and simplest code

---

## :material-file-code: Complete Working Example

For a complete working example demonstrating both approaches, see the sample project in the repository:
`samples/protocol-buffers-serialization/`

This sample includes:
- Complete project setup
- Protocol Buffers schema files
- Implementation of both approaches
- Unit tests
- Performance benchmarks

---

## :material-information: Additional Resources

- [Protocol Buffers Documentation](https://protobuf.dev/)
- [Google.Protobuf NuGet Package](https://www.nuget.org/packages/Google.Protobuf)
- [SharpVector Persistence Documentation](../persistence/index.md)
- [gRPC for .NET](https://grpc.io/docs/languages/csharp/)

---

## :material-frequently-asked-questions: FAQ

**Q: Is Protocol Buffers faster than SharpVector's native serialization?**

A: Not necessarily. SharpVector's native format is already binary and efficient. Protocol Buffers adds a layer that may slightly increase overhead unless you use Approach 2 (Native) which could be optimized for size.

**Q: Can I use Protocol Buffers with OpenAI-enabled databases?**

A: Yes! The serialization methods work with all implementations of `IVectorDatabase`, including `BasicOpenAIMemoryVectorDatabase` and `BasicOllamaMemoryVectorDatabase`.

**Q: Do I need gRPC to use Protocol Buffers?**

A: No. While Protocol Buffers and gRPC often work together, you can use Protocol Buffers for serialization without using gRPC for communication.

**Q: Can I serialize only part of the database?**

A: SharpVector serializes the entire database. If you need partial serialization, you would need to implement custom logic using Approach 2 (Native) and selectively include items.

**Q: Is the metadata preserved during Protocol Buffers serialization?**

A: Yes, both approaches preserve metadata. Approach 1 preserves it exactly as-is within the binary data. Approach 2 serializes it as JSON within the Protocol Buffers message.
