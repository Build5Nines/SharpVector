using Build5Nines.SharpVector;
using Google.Protobuf;
using SharpVectorProtobuf;

namespace ProtobufSerializationSample;

/// <summary>
/// Provides methods to serialize and deserialize SharpVector databases using Protocol Buffers.
/// This wraps SharpVector's native binary serialization in a Protocol Buffers message.
/// </summary>
public static class ProtobufVectorDatabaseSerializer
{
    /// <summary>
    /// Serializes a SharpVector database to Protocol Buffers format
    /// </summary>
    /// <typeparam name="TId">The ID type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="database">The database to serialize</param>
    /// <param name="databaseType">Optional database type identifier</param>
    /// <returns>Byte array containing the Protocol Buffers serialized data</returns>
    public static byte[] SerializeToProtobuf<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        string? databaseType = null)
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
            DatabaseType = databaseType ?? database.GetType().FullName ?? "Unknown",
            Version = "1.0",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        // Serialize to Protocol Buffers
        return wrapper.ToByteArray();
    }
    
    /// <summary>
    /// Deserializes a SharpVector database from Protocol Buffers format
    /// </summary>
    /// <typeparam name="TId">The ID type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="database">The database to deserialize into</param>
    /// <param name="protobufData">The Protocol Buffers serialized data</param>
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
    /// <typeparam name="TId">The ID type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="database">The database to serialize</param>
    /// <param name="databaseType">Optional database type identifier</param>
    /// <returns>Task containing byte array with Protocol Buffers serialized data</returns>
    public static async Task<byte[]> SerializeToProtobufAsync<TId, TMetadata>(
        IVectorDatabase<TId, TMetadata> database,
        string? databaseType = null)
        where TId : notnull
    {
        using var memoryStream = new MemoryStream();
        await database.SerializeToBinaryStreamAsync(memoryStream);
        
        var databaseData = memoryStream.ToArray();
        
        var wrapper = new VectorDatabaseWrapper
        {
            DatabaseData = ByteString.CopyFrom(databaseData),
            DatabaseType = databaseType ?? database.GetType().FullName ?? "Unknown",
            Version = "1.0",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        return wrapper.ToByteArray();
    }
    
    /// <summary>
    /// Async version: Deserializes a SharpVector database from Protocol Buffers format
    /// </summary>
    /// <typeparam name="TId">The ID type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="database">The database to deserialize into</param>
    /// <param name="protobufData">The Protocol Buffers serialized data</param>
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
    
    /// <summary>
    /// Gets metadata from a Protocol Buffers serialized database without fully deserializing it
    /// </summary>
    /// <param name="protobufData">The Protocol Buffers serialized data</param>
    /// <returns>Tuple containing database type, version, and timestamp</returns>
    public static (string DatabaseType, string Version, DateTimeOffset Timestamp) GetMetadata(byte[] protobufData)
    {
        var wrapper = VectorDatabaseWrapper.Parser.ParseFrom(protobufData);
        return (
            wrapper.DatabaseType,
            wrapper.Version,
            DateTimeOffset.FromUnixTimeSeconds(wrapper.Timestamp)
        );
    }
}
