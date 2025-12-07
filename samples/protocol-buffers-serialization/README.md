# Protocol Buffers Serialization Sample

This sample demonstrates how to serialize and deserialize SharpVector databases using Protocol Buffers (protobuf).

## Overview

Protocol Buffers is a language-neutral, platform-neutral extensible mechanism for serializing structured data. This sample shows how to:

1. Wrap SharpVector's native binary serialization in a Protocol Buffers message
2. Serialize a SharpVector database to Protocol Buffers format
3. Save the serialized data to a file
4. Load and deserialize the data back into a SharpVector database
5. Verify the data integrity after serialization/deserialization

## What's Included

- **`Protos/vectordb.proto`** - Protocol Buffers schema definition
- **`ProtobufVectorDatabaseSerializer.cs`** - Serialization utility class
- **`Program.cs`** - Complete demonstration of the serialization process

## Prerequisites

- .NET 8.0 SDK or later
- NuGet packages (automatically restored):
  - `Build5Nines.SharpVector`
  - `Google.Protobuf`
  - `Grpc.Tools`

## Running the Sample

```bash
cd samples/protocol-buffers-serialization/ProtobufSerializationSample
dotnet run
```

## Expected Output

The sample will:

1. Create a vector database with sample text entries
2. Perform a search to demonstrate functionality
3. Serialize the database to Protocol Buffers format
4. Display metadata from the serialized data
5. Save the data to a file (`vectordb.pb`)
6. Load the data from the file
7. Deserialize back into a new database
8. Verify the loaded database works correctly
9. Compare sizes between native and Protocol Buffers formats
10. Clean up temporary files

## Key Features Demonstrated

### Synchronous Operations

```csharp
// Serialize
var protobufData = ProtobufVectorDatabaseSerializer.SerializeToProtobuf(database);

// Deserialize
ProtobufVectorDatabaseSerializer.DeserializeFromProtobuf(loadedDatabase, protobufData);
```

### Asynchronous Operations

```csharp
// Serialize async
var protobufData = await ProtobufVectorDatabaseSerializer.SerializeToProtobufAsync(database);

// Deserialize async
await ProtobufVectorDatabaseSerializer.DeserializeFromProtobufAsync(loadedDatabase, protobufData);
```

### Metadata Extraction

```csharp
// Get metadata without full deserialization
var metadata = ProtobufVectorDatabaseSerializer.GetMetadata(protobufData);
Console.WriteLine($"Database Type: {metadata.DatabaseType}");
Console.WriteLine($"Version: {metadata.Version}");
Console.WriteLine($"Timestamp: {metadata.Timestamp}");
```

## Architecture

This sample uses the **wrapper approach**, which:

1. Serializes the database using SharpVector's native `SerializeToBinaryStream` method
2. Wraps the binary data in a Protocol Buffers message with additional metadata
3. Provides the benefits of Protocol Buffers (versioning, metadata) while maintaining full compatibility with SharpVector's format

### Protocol Buffers Schema

```protobuf
message VectorDatabaseWrapper {
  bytes database_data = 1;      // The serialized SharpVector data
  string database_type = 2;     // Type identifier
  string version = 3;           // Format version
  int64 timestamp = 4;          // Creation timestamp
}
```

## Use Cases

This approach is ideal for:

- **Microservices**: Send databases between services via gRPC
- **Cloud Storage**: Store databases with metadata in cloud storage systems
- **Caching**: Cache serialized databases with versioning information
- **Distribution**: Package and distribute pre-built vector databases
- **Cross-Platform**: Share databases across different .NET platforms

## Performance Considerations

The Protocol Buffers wrapper adds minimal overhead:

- **Size**: Typically 10-50 bytes of metadata overhead
- **Performance**: Negligible serialization/deserialization overhead
- **Compatibility**: 100% compatible with SharpVector's native format

## Customization

You can extend the Protocol Buffers schema to include additional metadata:

```protobuf
message VectorDatabaseWrapper {
  bytes database_data = 1;
  string database_type = 2;
  string version = 3;
  int64 timestamp = 4;
  
  // Add your custom fields
  string description = 5;
  map<string, string> tags = 6;
  int32 item_count = 7;
}
```

Then update the `ProtobufVectorDatabaseSerializer` class to populate these fields.

## Further Reading

- [Protocol Buffers Documentation](https://protobuf.dev/)
- [SharpVector Persistence Documentation](../../docs/docs/persistence/protocol-buffers.md)
- [Google.Protobuf API Reference](https://protobuf.dev/reference/csharp/api-docs/)

## License

This sample is part of the SharpVector project and is licensed under the MIT License.
