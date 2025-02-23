using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using System.Collections.Concurrent;
using OpenAI.Embeddings;

namespace Build5Nines.SharpVector.OpenAI;

public abstract class OpenAIMemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TIdGenerator, TVectorComparer>
    : IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVectorStore : IVectorStore<TId, TMetadata>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TVectorComparer : IVectorComparer, new()
{
    private TIdGenerator _idGenerator;

    private TVectorComparer _vectorComparer;

    /// <summary>
    /// The Vector Store used to store the text vectors of the database
    /// </summary>
    protected TVectorStore VectorStore { get; private set; }

    protected EmbeddingClient EmbeddingClient { get; private set; }

    public OpenAIMemoryVectorDatabaseBase(EmbeddingClient embeddingClient, TVectorStore vectorStore)
    {
        this.EmbeddingClient = embeddingClient;
        VectorStore = vectorStore;
        _idGenerator = new TIdGenerator();
        _vectorComparer = new TVectorComparer();
    }

    /// <summary>
    /// Get all the Ids for each text the database.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TId> GetIds()
    {
        return VectorStore.GetIds();
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public TId AddText(string text, TMetadata? metadata = default(TMetadata))
    {
        return AddTextAsync(text, metadata).Result;
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<TId> AddTextAsync(string text, TMetadata? metadata = default(TMetadata))
    {       
        // Generate the vector asynchronously
        var vector = await GetVectorsAsync(text);
        
        // Generate the ID and store the vector text item asynchronously
        TId id = _idGenerator.NewId();
        await VectorStore.SetAsync(id, new VectorTextItem<TMetadata>(text, metadata, vector));
        
        return id;
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<TMetadata> GetText(TId id)
    {
        return VectorStore.Get(id);
    }

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<TMetadata> DeleteText(TId id)
    {
        return VectorStore.Delete(id);
    }

    private async Task<float[]> GetVectorsAsync(string text)
    {
        var result = await this.EmbeddingClient.GenerateEmbeddingAsync(text);
        var embedding = result.Value;
        var vector = embedding.ToFloats();
        return vector.ToArray();
    }

    private float[] GetVectors(string text)
    {
        var task = GetVectorsAsync(text);
        task.Wait();
        return task.Result;
    }

    /// <summary>
    /// Updates a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task UpdateTextAsync(TId id, string text)
    {
        if (VectorStore.ContainsKey(id))
        {
            var vector = await GetVectorsAsync(text);
            var metadata = VectorStore.Get(id).Metadata;
            VectorStore.Set(id, new VectorTextItem<TMetadata>(text, metadata, vector));
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
    }

    /// <summary>
    /// Updates a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    public void UpdateText(TId id, string text)
    {
        var task = UpdateTextAsync(id, text);
        task.Wait();
    }

    /// <summary>
    /// Updates the Metadata of a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="metadata"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateTextMetadata(TId id, TMetadata metadata) {
        if (VectorStore.ContainsKey(id))
        {
            var text = GetText(id);
            text.Metadata = metadata;
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
    }

    /// <summary>
    /// Updates a Text by its ID with new text and metadata values
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <param name="metadata"></param>
    public async Task UpdateTextAndMetadataAsync(TId id, string text, TMetadata metadata)
    {
        if (VectorStore.ContainsKey(id))
        {
            var vector = await GetVectorsAsync(text);
            VectorStore.Set(id, new VectorTextItem<TMetadata>(text, metadata, vector));
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
    }

    /// <summary>
    /// Updates a Text by its ID with new text and metadata values
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <param name="metadata"></param>
    public void UpdateTextAndMetadata(TId id, string text, TMetadata metadata)
    {
        var task = UpdateTextAndMetadataAsync(id, text, metadata);
        task.Wait();
    }

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="topN">The highest number of results to show.</param>
    /// <param name="threshold">The similarity threshold. Only return items greater or equal to the threshold. Null returns all.</param>
    /// <returns></returns>
    public IVectorTextResult<TMetadata> Search(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
    {
        return SearchAsync(queryText, threshold, pageIndex, pageCount).Result;
    }

    /// <summary>
    /// Performs an asynchronous search vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <returns></returns>
    public async Task<IVectorTextResult<TMetadata>> SearchAsync(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
    {
        var similarities = await CalculateVectorComparisonAsync(queryText, threshold);

        similarities = await _vectorComparer.SortAsync(similarities);

        var totalCountFoundInSearch = similarities.Count();

        IEnumerable<VectorTextResultItem<TMetadata>> resultsToReturn;
        if (pageCount != null && pageCount >= 0 && pageIndex >= 0) {
            resultsToReturn = similarities.Skip(pageIndex * pageCount.Value).Take(pageCount.Value);
        } else {
            // no paging specified, return all results
            resultsToReturn = similarities;
        }

        return new VectorTextResult<TMetadata>(totalCountFoundInSearch, pageIndex, pageCount.HasValue ? pageCount.Value : 1, resultsToReturn);
    }

    private async Task<IEnumerable<VectorTextResultItem<TMetadata>>> CalculateVectorComparisonAsync(string queryText, float? threshold = null)
    {
        var queryVector = await GetVectorsAsync(queryText);

        if (VectorStore.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var results = new ConcurrentBag<VectorTextResultItem<TMetadata>>();
        await foreach (var kvp in VectorStore)
        {
            var item = kvp.Value;
            //float vectorComparisonValue = await _vectorComparer.CalculateAsync(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));
            float vectorComparisonValue = await _vectorComparer.CalculateAsync(queryVector, item.Vector);

            if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
            {
                results.Add(new VectorTextResultItem<TMetadata>(item, vectorComparisonValue));
            }
        }
        return results;
    }

        /// <summary>
    /// Serializes the Vector Database to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task SerializeToJsonStreamAsync(Stream stream)
    {
        var streamVectorStore = new MemoryStream();
        var streamVocabularyStore = new MemoryStream();

        var taskVectorStore = VectorStore.SerializeToJsonStreamAsync(streamVectorStore);
        var taskVocabularyStore = VectorStore.VocabularyStore.SerializeToJsonStreamAsync(streamVocabularyStore);

        await Task.WhenAll(taskVectorStore, taskVocabularyStore);

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            var entryDatabaseType = archive.CreateEntry("database.json");
            using (var entryStream = entryDatabaseType.Open())
            {
                var databaseInfo = new DatabaseInfo(this.GetType().FullName);

                var databaseInfoJson = JsonSerializer.Serialize(databaseInfo);

                if (databaseInfoJson != null)
                {
                    var databaseTypeBytes = System.Text.Encoding.UTF8.GetBytes(databaseInfoJson);
                    await entryStream.WriteAsync(databaseTypeBytes);
                    await entryStream.FlushAsync();
                }
                else
                {
                    throw new InvalidOperationException("Type name cannot be null.");
                }
            }
            var entryVectorStore = archive.CreateEntry("vectorstore.json");
            using (var entryStream = entryVectorStore.Open())
            {
                streamVectorStore.Position = 0;
                await streamVectorStore.CopyToAsync(entryStream);
                await entryStream.FlushAsync();
            }
        }

        await stream.FlushAsync();
    }

    public virtual void SerializeToJsonStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        SerializeToJsonStreamAsync(stream).Wait();
    }

    public virtual async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            var entryDatabaseType = archive.GetEntry("database.json");
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

                    if (databaseInfo.Schema != DatabaseInfo.SupportedSchema)
                    {
                        throw new DatabaseFileSchemaException($"The database schema does not match the expected schema (Expected: {DatabaseInfo.SupportedSchema} - Actual: {databaseInfo.Schema})."); 
                    }

                    if (databaseInfo.Version != DatabaseInfo.SupportedVersion)
                    {
                        throw new DatabaseFileVersionException($"The database version does not match the expected version (Expected: {DatabaseInfo.SupportedVersion} - Actual: {databaseInfo.Version}).");
                    }

                    if (databaseInfo.ClassType != this.GetType().FullName)
                    {
                        throw new DatabaseFileClassTypeException($"The database class type does not match the expected type (Expected: {this.GetType().FullName} - Actual: {databaseInfo.ClassType})");
                    }
                }
            }
            else
            {
                throw new DatabaseFileMissingEntryException("Database info entry not found.", "database");
            }


            var entryVectorStore = archive.GetEntry("vectorstore.json");
            if (entryVectorStore != null)
            {
                using (var entryStream = entryVectorStore.Open())
                {
                    await VectorStore.DeserializeFromJsonStreamAsync(entryStream);
                }
            }
            else
            {
                throw new DatabaseFileMissingEntryException("Vector Store entry not found.", "vectorstore");
            }
        }
    }

    public virtual void DeserializeFromJsonStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        DeserializeFromJsonStreamAsync(stream).Wait();
    }

}