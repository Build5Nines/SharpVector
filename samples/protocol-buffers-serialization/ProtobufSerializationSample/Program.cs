using Build5Nines.SharpVector;
using ProtobufSerializationSample;

Console.WriteLine("=== SharpVector Protocol Buffers Serialization Demo ===\n");

// Step 1: Create and populate a vector database
Console.WriteLine("Step 1: Creating and populating vector database...");
var database = new BasicMemoryVectorDatabase();

database.AddText("Artificial intelligence and machine learning are transforming technology.", "AI");
database.AddText("Protocol Buffers provide efficient serialization for structured data.", "Protobuf");
database.AddText("Vector databases enable semantic search capabilities.", "VectorDB");
database.AddText("The SharpVector library is a lightweight in-memory vector database.", "SharpVector");
database.AddText("Cloud computing services provide scalable infrastructure.", "Cloud");

Console.WriteLine($"   Added {database.GetIds().Count()} items to the database.\n");

// Step 2: Perform a search before serialization
Console.WriteLine("Step 2: Testing search before serialization...");
var searchResults = database.Search("machine learning artificial intelligence");
Console.WriteLine($"   Search query: 'machine learning artificial intelligence'");
Console.WriteLine($"   Found {searchResults.TotalCount} results:");
foreach (var result in searchResults.Texts.Take(3))
{
    Console.WriteLine($"      - [{result.Id}] {result.Text} (Similarity: {result.Similarity:F4})");
}
Console.WriteLine();

// Step 3: Serialize to Protocol Buffers
Console.WriteLine("Step 3: Serializing database to Protocol Buffers format...");
var protobufData = ProtobufVectorDatabaseSerializer.SerializeToProtobuf(database);
Console.WriteLine($"   Serialized to {protobufData.Length:N0} bytes.\n");

// Step 4: Get metadata from serialized data
Console.WriteLine("Step 4: Reading metadata from serialized data...");
var metadata = ProtobufVectorDatabaseSerializer.GetMetadata(protobufData);
Console.WriteLine($"   Database Type: {metadata.DatabaseType}");
Console.WriteLine($"   Version: {metadata.Version}");
Console.WriteLine($"   Timestamp: {metadata.Timestamp:yyyy-MM-dd HH:mm:ss} UTC\n");

// Step 5: Save to file
var filePath = "vectordb.pb";
Console.WriteLine($"Step 5: Saving Protocol Buffers data to file '{filePath}'...");
File.WriteAllBytes(filePath, protobufData);
Console.WriteLine($"   Saved {new FileInfo(filePath).Length:N0} bytes to disk.\n");

// Step 6: Load from file and deserialize
Console.WriteLine("Step 6: Loading from file and deserializing...");
var loadedProtobufData = File.ReadAllBytes(filePath);
var loadedDatabase = new BasicMemoryVectorDatabase();
ProtobufVectorDatabaseSerializer.DeserializeFromProtobuf(loadedDatabase, loadedProtobufData);
Console.WriteLine($"   Loaded {loadedDatabase.GetIds().Count()} items from file.\n");

// Step 7: Verify the loaded database works correctly
Console.WriteLine("Step 7: Verifying loaded database with search...");
var verifyResults = loadedDatabase.Search("machine learning artificial intelligence");
Console.WriteLine($"   Search query: 'machine learning artificial intelligence'");
Console.WriteLine($"   Found {verifyResults.TotalCount} results:");
foreach (var result in verifyResults.Texts.Take(3))
{
    Console.WriteLine($"      - [{result.Id}] {result.Text} (Similarity: {result.Similarity:F4})");
}
Console.WriteLine();

// Step 8: Demonstrate async serialization
Console.WriteLine("Step 8: Testing async serialization and deserialization...");
var asyncProtobufData = await ProtobufVectorDatabaseSerializer.SerializeToProtobufAsync(database);
var asyncDatabase = new BasicMemoryVectorDatabase();
await ProtobufVectorDatabaseSerializer.DeserializeFromProtobufAsync(asyncDatabase, asyncProtobufData);
Console.WriteLine($"   Async operations completed successfully.");
Console.WriteLine($"   Async database contains {asyncDatabase.GetIds().Count()} items.\n");

// Step 9: Compare sizes
Console.WriteLine("Step 9: Comparing serialization formats...");
using var nativeStream = new MemoryStream();
database.SerializeToBinaryStream(nativeStream);
var nativeSize = nativeStream.Length;
var protobufSize = protobufData.Length;
Console.WriteLine($"   Native SharpVector format: {nativeSize:N0} bytes");
Console.WriteLine($"   Protocol Buffers wrapper:  {protobufSize:N0} bytes");
Console.WriteLine($"   Overhead: {protobufSize - nativeSize:N0} bytes ({((double)(protobufSize - nativeSize) / nativeSize * 100):F2}%)\n");

// Cleanup
if (File.Exists(filePath))
{
    File.Delete(filePath);
    Console.WriteLine($"Cleanup: Deleted '{filePath}'");
}

Console.WriteLine("\n=== Demo completed successfully! ===");
