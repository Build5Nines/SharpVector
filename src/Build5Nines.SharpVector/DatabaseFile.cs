
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

namespace Build5Nines.SharpVector;

public static class DatabaseFile
{
    private const string databaseInfoFilename = "database.json";
    private const string vectorStoreFilename = "vectorstore.json";
    private const string vocabularyStoreFilename = "vocabularystore.json";

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

    /// <summary>
    /// Load the vector database from a file path
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<DatabaseInfo> LoadDatabaseInfoAsync(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            return await LoadDatabaseInfoFromZipArchiveAsync(stream);
        }
    }

    /// <summary>
    /// Load the vector database from a stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<DatabaseInfo> LoadDatabaseInfoFromZipArchiveAsync(Stream stream)
    {       
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
           return await LoadDatabaseInfoAsync(archive);
        }
    }

    /// <summary>
    /// Load the vector database info from a stream of JSON data
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseFileInfoException"></exception>
    public static async Task<DatabaseInfo> LoadDatabaseInfoFromJsonAsync(Stream stream)
    {
        var databaseInfo = await JsonSerializer.DeserializeAsync<DatabaseInfo>(stream);

        if (databaseInfo == null)
        {
            throw new DatabaseFileInfoException("Database info entry is null.");
        }

        return databaseInfo;
    }

    /// <summary>
    /// Load the vector database info from a ZipArchive
    /// </summary>
    /// <param name="archive"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseFileInfoException"></exception>
    /// <exception cref="DatabaseFileMissingEntryException"></exception>
    public static async Task<DatabaseInfo> LoadDatabaseInfoAsync(ZipArchive archive)
    {
        using(var entryStream = GetArchiveFilestream(archive, databaseInfoFilename, "database"))
        {
            return await LoadDatabaseInfoFromJsonAsync(entryStream);
        }
    }

    private static Stream GetArchiveFilestream(ZipArchive archive, string filename, string databaseEntryName)
    {
        var entryType = archive.GetEntry(filename);
        if (entryType != null)
        {
            var entryStream = entryType.Open();
            return entryStream;
        } else {
            throw new DatabaseFileMissingEntryException("Database entry not found.", databaseEntryName);
        }
    }

    public static async Task LoadVectorStoreAsync<TId, TMetadata, TDocument>(
        ZipArchive archive, IVectorStore<TId, TMetadata, TDocument> vectorStore
        )

    {
        using(var entryStream = GetArchiveFilestream(archive, vectorStoreFilename, "vectorstore"))
        {
            await vectorStore.DeserializeFromJsonStreamAsync(entryStream);
        }
    }

    public static async Task LoadVocabularyStoreAsync<TVocabularyKey, TVocabularyValue>(
        ZipArchive archive, IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore
        )
        where TVocabularyKey : notnull
    {
        using(var entryStream = GetArchiveFilestream(archive, vocabularyStoreFilename, "vocabularystore"))
        {
            await vocabularyStore.DeserializeFromJsonStreamAsync(entryStream);
        }
    }

    public static async Task<DatabaseInfo> LoadDatabaseFromZipArchiveAsync(
        Stream stream, string? dbClassType,
        Func<ZipArchive, Task> loadVectorStore
        )
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
           var databaseInfo = await LoadDatabaseInfoAsync(archive);

            if (databaseInfo.Schema != DatabaseInfo.SupportedSchema)
            {
                throw new DatabaseFileSchemaException($"The database schema does not match the expected schema (Expected: {DatabaseInfo.SupportedSchema} - Actual: {databaseInfo.Schema})."); 
            }

            if (databaseInfo.Version != DatabaseInfo.SupportedVersion)
            {
                throw new DatabaseFileVersionException($"The database version does not match the expected version (Expected: {DatabaseInfo.SupportedVersion} - Actual: {databaseInfo.Version}).");
            }

            if (databaseInfo.ClassType != dbClassType)
            {
                throw new DatabaseFileClassTypeException($"The database class type does not match the expected type (Expected: {dbClassType} - Actual: {databaseInfo.ClassType})");
            }

            // Load the vector store
            await loadVectorStore(archive);

            return databaseInfo;
        }
    }
}