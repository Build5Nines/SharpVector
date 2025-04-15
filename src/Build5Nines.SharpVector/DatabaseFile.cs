
using System.IO.Compression;
using System.Text.Json;

namespace Build5Nines.SharpVector;

public static class DatabaseFile
{
    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<MemoryVectorDatabase<TMetadata>> Load<TMetadata>(Stream stream)
    {
        return await Load<MemoryVectorDatabase<TMetadata>, TMetadata>(stream);
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <typeparam name="TVectorDatabase"></typeparam>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TMetadata>(Stream stream)
        where TVectorDatabase : MemoryVectorDatabase<TMetadata>, new()
    {
        return await Load<TVectorDatabase, int, TMetadata, string>(stream);   
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TId, TMetadata, TDocument>(Stream stream)
        where TVectorDatabase : IVectorDatabase<TId, TMetadata, TDocument>, new()
        where TId : notnull
    {
        var vdb = new TVectorDatabase();
        return await Load<TVectorDatabase, TId, TMetadata, TDocument>(vdb, stream);
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <param name="vdb"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TId, TMetadata, TDocument>(TVectorDatabase vdb, Stream stream)
        where TVectorDatabase : IVectorDatabase<TId, TMetadata, TDocument>
        where TId : notnull
    {
        await vdb.DeserializeFromBinaryStreamAsync(stream);
        return vdb;
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<MemoryVectorDatabase<TMetadata>> Load<TMetadata>(string filePath)
    {
        return await Load<MemoryVectorDatabase<TMetadata>, TMetadata>(filePath);
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <typeparam name="TVectorDatabase"></typeparam>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TMetadata>(string filePath)
        where TVectorDatabase : MemoryVectorDatabase<TMetadata>, new()
    {
        return await Load<TVectorDatabase, int, TMetadata, string>(filePath);   
    }

    /// <summary>
    ///  Load the vector database from a file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TId, TMetadata, TDocument>(string filePath)
        where TVectorDatabase: IVectorDatabase<TId, TMetadata, TDocument>, new()
        where TId : notnull
    {
        var vdb = new TVectorDatabase();
        return await Load<TVectorDatabase, TId, TMetadata, TDocument>(vdb, filePath);
    }

    /// <summary>
    /// Load the vector database from a file
    /// </summary>
    /// <param name="vdb"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<TVectorDatabase> Load<TVectorDatabase, TId, TMetadata, TDocument>(TVectorDatabase vdb, string filePath)
        where TVectorDatabase : IVectorDatabase<TId, TMetadata, TDocument>
        where TId : notnull
    {
        await vdb.LoadFromFileAsync(filePath);
        return vdb;
    }

    public static async Task<DatabaseInfo> LoadDatabaseInfoAsync(Stream stream)
    {
        const string databaseFilename = "database.json";
        
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            var entryDatabaseType = archive.GetEntry(databaseFilename);
            if (entryDatabaseType != null)
            {
                using (var entryStream = entryDatabaseType.Open())
                {
                    var databaseTypeStream = new MemoryStream();
                    await entryStream.CopyToAsync(databaseTypeStream);
                    databaseTypeStream.Position = 0;

                    var databaseTypeBytes = new byte[databaseTypeStream.Length];
                    await databaseTypeStream.ReadAsync(databaseTypeBytes);
                    var databaseInfoJson = System.Text.Encoding.UTF8.GetString(databaseTypeBytes);

                    var databaseInfo = JsonSerializer.Deserialize<DatabaseInfo>(databaseInfoJson);

                    if (databaseInfo == null)
                    {
                        throw new DatabaseFileInfoException("Database info entry is null.");
                    }

                    return databaseInfo;
                }
            }
            else
            {
                throw new DatabaseFileMissingEntryException("Database info entry not found.", "database");
            }
        }
    }

    public static async Task<DatabaseInfo> LoadDatabaseInfoAsync(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            return await LoadDatabaseInfoAsync(stream);
        }
    }
}