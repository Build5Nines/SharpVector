using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Build5Nines.SharpVector.Embeddings;
using System.Runtime.ExceptionServices;
using System.Collections;
using System.Linq;

namespace Build5Nines.SharpVector;


/// <summary>
/// Base class for a memory vector database.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVectorStore"></typeparam>
/// <typeparam name="TVocabularyStore"></typeparam>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
/// <typeparam name="TIdGenerator"></typeparam>
/// <typeparam name="TTextPreprocessor"></typeparam>
/// <typeparam name="TVectorizer"></typeparam>
/// <typeparam name="TVectorComparer"></typeparam>
public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TVocabularyStore, TVocabularyKey, TVocabularyValue, TIdGenerator, TTextPreprocessor, TVectorizer, TVectorComparer>
    : IVectorDatabase<TId, TMetadata, TVocabularyKey>
    where TId : notnull
    where TVocabularyKey : notnull
    where TVocabularyValue: notnull
    where TVectorStore : IVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
    where TVocabularyStore : IVocabularyStore<TVocabularyKey, TVocabularyValue>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TTextPreprocessor : ITextPreprocessor<TVocabularyKey>, new()
    where TVectorizer : IVectorizer<TVocabularyKey, TVocabularyValue>, new()
    where TVectorComparer : IVectorComparer, new()
{
    protected TIdGenerator _idGenerator;

    private TTextPreprocessor _textPreprocessor;

    private TVectorizer _vectorizer;

    private TVectorComparer _vectorComparer;

    /// <summary>
    /// The Vector Store used to store the text vectors of the database
    /// </summary>
    protected TVectorStore VectorStore { get; private set; }

    public MemoryVectorDatabaseBase(TVectorStore vectorStore)
    {
        VectorStore = vectorStore;
        _idGenerator = new TIdGenerator();
        _textPreprocessor = new TTextPreprocessor();
        _vectorizer = new TVectorizer();
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
    public TId AddText(TVocabularyKey text, TMetadata? metadata = default(TMetadata))
    {
        return AddTextAsync(text, metadata).Result;
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<TId> AddTextAsync(TVocabularyKey text, TMetadata? metadata = default(TMetadata))
    {
        // Perform preprocessing asynchronously
        var tokens = await _textPreprocessor.TokenizeAndPreprocessAsync(text);
        
        // Update the vocabulary store asynchronously
        await VectorStore.VocabularyStore.UpdateAsync(tokens);
        
        // Generate the vector asynchronously
        float[] vector = await _vectorizer.GenerateVectorFromTokensAsync(VectorStore.VocabularyStore, tokens);
        
        // Generate the ID and store the vector text item asynchronously
        TId id = _idGenerator.NewId();
        await VectorStore.SetAsync(id, new VectorTextItem<TVocabularyKey, TMetadata>(text, metadata, vector));
        
        return id;
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<TVocabularyKey, TMetadata> GetText(TId id)
    {
        return VectorStore.Get(id);
    }

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<TVocabularyKey, TMetadata> DeleteText(TId id)
    {
        return VectorStore.Delete(id);
    }

    /// <summary>
    /// Updates a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateText(TId id, TVocabularyKey text)
    {
        if (VectorStore.ContainsKey(id))
        {
            var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
            VectorStore.VocabularyStore.Update(tokens);
            float[] vector = _vectorizer.GenerateVectorFromTokens(VectorStore.VocabularyStore, tokens);
            var metadata = VectorStore.Get(id).Metadata;
            VectorStore.Set(id, new VectorTextItem<TVocabularyKey, TMetadata>(text, metadata, vector));
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
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
            var existing = VectorStore.Get(id);

            var item = new VectorTextItem<TVocabularyKey, TMetadata>(
                existing.Text,
                metadata,
                existing.Vector
            );
            
            VectorStore.Set(id, item);
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
    public void UpdateTextAndMetadata(TId id, TVocabularyKey text, TMetadata metadata)
    {
        if (VectorStore.ContainsKey(id))
        {
            var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
            VectorStore.VocabularyStore.Update(tokens);
            float[] vector = _vectorizer.GenerateVectorFromTokens(VectorStore.VocabularyStore, tokens);
            VectorStore.Set(id, new VectorTextItem<TVocabularyKey, TMetadata>(text, metadata, vector));
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
    }

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="topN">The highest number of results to show.</param>
    /// <param name="threshold">The similarity threshold. Only return items greater or equal to the threshold. Null returns all.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <param name="filter">A filter function to apply to the metadata of each result.</param>
    /// <returns></returns>
    public IVectorTextResult<TId, TVocabularyKey, TMetadata> Search(TVocabularyKey queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null, Func<TMetadata?, bool>? filter = null)
    {
        Func<TMetadata?, Task<bool>>? filterToUse = null;
        if (filter != null)
        {
            filterToUse = (metadata) => Task.FromResult(filter(metadata));
        }
        return SearchAsync(queryText, threshold, pageIndex, pageCount, filterToUse).Result;
    }

    /// <summary>
    /// Performs an asynchronous search vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <param name="filter">A filter function to apply to the metadata of each result.</param>
    /// <returns></returns>
    public async Task<IVectorTextResult<TId, TVocabularyKey, TMetadata>> SearchAsync(TVocabularyKey queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null, Func<TMetadata?, Task<bool>>? filter = null)
    {
        var similarities = await CalculateVectorComparisonAsync(queryText, threshold, filter);

        similarities = await _vectorComparer.SortAsync(similarities);

        var totalCountFoundInSearch = similarities.Count();

        IEnumerable<IVectorTextResultItem<TId, TVocabularyKey, TMetadata>> resultsToReturn;
        if (pageCount != null && pageCount >= 0 && pageIndex >= 0) {
            resultsToReturn = similarities.Skip(pageIndex * pageCount.Value).Take(pageCount.Value);
        } else {
            // no paging specified, return all results
            resultsToReturn = similarities;
        }

        return new VectorTextResult<TId, TVocabularyKey, TMetadata>(totalCountFoundInSearch, pageIndex, pageCount.HasValue ? pageCount.Value : 1, resultsToReturn);
    }

    private async Task<IEnumerable<IVectorTextResultItem<TId, TVocabularyKey, TMetadata>>> CalculateVectorComparisonAsync(TVocabularyKey queryText, float? threshold = null, Func<TMetadata?, Task<bool>>? filter = null)
    {
        var queryTokens = _textPreprocessor.TokenizeAndPreprocess(queryText);
        float[] queryVector = _vectorizer.GenerateVectorFromTokens(VectorStore.VocabularyStore, queryTokens);

        // Method to get the maximum vector length in the database
        var desiredLength = VectorStore.VocabularyStore.Count;

        if (VectorStore.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var results = new ConcurrentBag<VectorTextResultItem<TId, TVocabularyKey, TMetadata>>();
        await foreach (KeyValuePair<TId, VectorTextItem<TVocabularyKey, TMetadata>> kvp in VectorStore)
        {
            if (filter == null || await filter(kvp.Value.Metadata))
            {
                var item = kvp.Value;
                float vectorComparisonValue = await _vectorComparer.CalculateAsync(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));

                if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
                {
                    var id = kvp.Key;
                    results.Add(
                        new VectorTextResultItem<TId, TVocabularyKey, TMetadata>(id, item, vectorComparisonValue)
                        );
                }
            }
        }
        return results;
    }

    [Obsolete("Use SerializeToBinaryStreamAsync instead.")]
    public virtual async Task SerializeToJsonStreamAsync(Stream stream)
    {
        await SerializeToBinaryStreamAsync(stream);
    }
    
    /// <summary>
    /// Serializes the Vector Database to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task SerializeToBinaryStreamAsync(Stream stream)
    {
        var streamVectorStore = new MemoryStream();
        var streamVocabularyStore = new MemoryStream();

        var taskVectorStore = VectorStore.SerializeToJsonStreamAsync(streamVectorStore);
        var taskVocabularyStore = VectorStore.VocabularyStore.SerializeToJsonStreamAsync(streamVocabularyStore);

        await Task.WhenAll(taskVectorStore, taskVocabularyStore);

        await DatabaseFile.SaveDatabaseToZipArchiveAsync(
            stream,
            new DatabaseInfo(this.GetType().FullName),
            async (archive) =>
            {
                var entryVectorStore = archive.CreateEntry(DatabaseFile.vectorStoreFilename);
                using (var entryStream = entryVectorStore.Open())
                {
                    streamVectorStore.Position = 0;
                    await streamVectorStore.CopyToAsync(entryStream);
                    await entryStream.FlushAsync();
                }

                var entryVocabularyStore = archive.CreateEntry(DatabaseFile.vocabularyStoreFilename);
                using (var entryStream = entryVocabularyStore.Open())
                {
                    streamVocabularyStore.Position = 0;
                    await streamVocabularyStore.CopyToAsync(entryStream);
                    await entryStream.FlushAsync();
                }
            }
        );
        await stream.FlushAsync();
    }

    [Obsolete("Use SerializeToBinaryStream instead.")]
    public virtual void SerializeToJsonStream(Stream stream)
    {
        SerializeToBinaryStream(stream);    
    }

    public virtual void SerializeToBinaryStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        SerializeToBinaryStreamAsync(stream).Wait();
    }

    [Obsolete("Use DeserializeFromBinaryStreamAsync instead.")]
    public virtual async Task DeserializeFromJsonStreamAsync(Stream stream) {
        await DeserializeFromBinaryStreamAsync(stream);
    }

    public virtual async Task DeserializeFromBinaryStreamAsync(Stream stream)
    {
        await DatabaseFile.LoadDatabaseFromZipArchiveAsync(
            stream,
            this.GetType().FullName,
            async (archive) =>
            {
                await DatabaseFile.LoadVectorStoreAsync(archive, VectorStore);
                await DatabaseFile.LoadVocabularyStoreAsync(archive, VectorStore.VocabularyStore);

                // Re-initialize the IdGenerator with the max Id value from the VectorStore if it supports sequential numeric IDs
                if (_idGenerator is ISequentialIdGenerator<TId> seqIdGen)
                {
                    // Re-seed the sequence only if there are existing IDs
                    var ids = VectorStore.GetIds();
                    if (ids.Any())
                    {
                        seqIdGen.SetMostRecent(ids.Max()!);
                    }
                }
            }
        );
    }


    [Obsolete("Use DeserializeFromBinaryStream instead")]
    public virtual void DeserializeFromJsonStream(Stream stream)
    {
        DeserializeFromBinaryStream(stream);
    }

    public virtual void DeserializeFromBinaryStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        DeserializeFromBinaryStreamAsync(stream).Wait();
    }

    public IEnumerator<IVectorTextDatabaseItem<TId, TVocabularyKey, TMetadata>> GetEnumerator()
    {
        return VectorStore.Select(kvp => new VectorTextDatabaseItem<TId, TVocabularyKey, TMetadata>(kvp.Key, kvp.Value.Text, kvp.Value.Metadata, kvp.Value.Vector)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}



/// <summary>
/// Base class for a memory vector database.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVectorStore"></typeparam>
/// <typeparam name="TIdGenerator"></typeparam>
/// <typeparam name="TVectorComparer"></typeparam>
public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TIdGenerator, TVectorComparer>
    : IMemoryVectorDatabase<TId, TMetadata>, IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVectorStore : IVectorStore<TId, TMetadata, string>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TVectorComparer : IVectorComparer, new()
{
    private TIdGenerator _idGenerator;

    private TVectorComparer _vectorComparer;

    /// <summary>
    /// The Vector Store used to store the text vectors of the database
    /// </summary>
    protected TVectorStore VectorStore { get; private set; }

    protected IEmbeddingsGenerator EmbeddingsGenerator { get; private set; }

    public MemoryVectorDatabaseBase(IEmbeddingsGenerator embeddingsGenerator, TVectorStore vectorStore)
    {
        EmbeddingsGenerator = embeddingsGenerator;
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
        var vector = await EmbeddingsGenerator.GenerateEmbeddingsAsync(text);
        
        // Generate the ID and store the vector text item asynchronously
        TId id = _idGenerator.NewId();
        await VectorStore.SetAsync(id, new VectorTextItem<TMetadata>(text, metadata, vector));
        
        return id;
    }

    /// <summary>
    /// Adds multiple texts with optional metadata to the database efficiently.
    /// If the embeddings generator supports batching, this will generate vectors in a single multi-input call.
    /// </summary>
    /// <param name="items">Collection of (text, metadata) tuples to add.</param>
    /// <returns>List of generated IDs in the same order as inputs.</returns>
    public async Task<IReadOnlyList<TId>> AddTextsAsync(IEnumerable<(string text, TMetadata? metadata)> items)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        var list = items.ToList();
        if (list.Count == 0) return Array.Empty<TId>();

        // Try batch embeddings if supported
        float[][] vectors;
        if (EmbeddingsGenerator is IBatchEmbeddingsGenerator batchGen)
        {
            var batch = await batchGen.GenerateEmbeddingsAsync(list.Select(i => i.text));
            vectors = batch.Select(v => v.ToArray()).ToArray();
        }
        else
        {
            // Fallback to per-item embedding
            vectors = new float[list.Count][];
            for (int i = 0; i < list.Count; i++)
            {
                vectors[i] = await EmbeddingsGenerator.GenerateEmbeddingsAsync(list[i].text);
            }
        }

        // Store items and produce IDs
        var ids = new List<TId>(list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            TId id = _idGenerator.NewId();
            ids.Add(id);
            await VectorStore.SetAsync(id, new VectorTextItem<TMetadata>(list[i].text, list[i].metadata, vectors[i]));
        }

        return ids;
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<string, TMetadata> GetText(TId id)
    {
        return VectorStore.Get(id);
    }

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<string, TMetadata> DeleteText(TId id)
    {
        return VectorStore.Delete(id);
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
            var existing = VectorStore.Get(id);
            var vector = await EmbeddingsGenerator.GenerateEmbeddingsAsync(text);
            var metadata = existing.Metadata;
            VectorStore.Set(id, new VectorTextItem<TMetadata>(text, existing.Metadata, vector));
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
            var existing = VectorStore.Get(id);

            var item = new VectorTextItem<string, TMetadata>(
                existing.Text,
                metadata,
                existing.Vector
            );
            
            VectorStore.Set(id, item);
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
            var vector = await EmbeddingsGenerator.GenerateEmbeddingsAsync(text);
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
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <param name="filter">A filter function to apply to the metadata of each result.</param>
    /// <returns>The search results as an IVectorTextResult object.</returns>
    public IVectorTextResult<TId, string, TMetadata> Search(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null, Func<TMetadata?, bool>? filter = null)
    {
        Func<TMetadata?, Task<bool>>? filterToUse = null;
        if (filter != null)
        {
            filterToUse = (metadata) => Task.FromResult(filter(metadata));
        }
        return SearchAsync(queryText, threshold, pageIndex, pageCount, filterToUse).Result;
    }

    /// <summary>
    /// Performs an asynchronous search vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <param name="filter">A filter function to apply to the metadata of each result.</param>
    /// <returns>The search results as an IVectorTextResult object.</returns>
    public async Task<IVectorTextResult<TId, string, TMetadata>> SearchAsync(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null, Func<TMetadata?, Task<bool>>? filter = null)
    {
        var similarities = await CalculateVectorComparisonAsync(queryText, threshold, filter);

        similarities = await _vectorComparer.SortAsync(similarities);

        var totalCountFoundInSearch = similarities.Count();

        IEnumerable<IVectorTextResultItem<TId, string, TMetadata>> resultsToReturn;
        if (pageCount != null && pageCount >= 0 && pageIndex >= 0) {
            resultsToReturn = similarities.Skip(pageIndex * pageCount.Value).Take(pageCount.Value);
        } else {
            // no paging specified, return all results
            resultsToReturn = similarities;
        }

        return new VectorTextResult<TId, string, TMetadata>(totalCountFoundInSearch, pageIndex, pageCount.HasValue ? pageCount.Value : 1, resultsToReturn);
    }

    private async Task<IEnumerable<IVectorTextResultItem<TId, string, TMetadata>>> CalculateVectorComparisonAsync(string queryText, float? threshold = null, Func<TMetadata?, Task<bool>>? filter = null)
    {
        var queryVector = await EmbeddingsGenerator.GenerateEmbeddingsAsync(queryText);

        if (VectorStore.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var results = new ConcurrentBag<VectorTextResultItem<TId, string, TMetadata>>();
        await foreach (var kvp in VectorStore)
        {
            if (filter == null || await filter(kvp.Value.Metadata))
            {
                var item = kvp.Value;

                float vectorComparisonValue = await _vectorComparer.CalculateAsync(queryVector, item.Vector);

                if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
                {
                    var id = kvp.Key;
                    results.Add(
                        new VectorTextResultItem<TId, string, TMetadata>(id, item, vectorComparisonValue)
                        );
                }
            }
        }
        return results;
    }

    [Obsolete("Use SerializeToBinaryStreamAsync Instead")]
    public virtual async Task SerializeToJsonStreamAsync(Stream stream)
    {
        await SerializeToBinaryStreamAsync(stream);
    }

    /// <summary>
    /// Serializes the Vector Database to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task SerializeToBinaryStreamAsync(Stream stream)
    {
        var streamVectorStore = new MemoryStream();
        await VectorStore.SerializeToJsonStreamAsync(streamVectorStore);

        await DatabaseFile.SaveDatabaseToZipArchiveAsync(
            stream,
            new DatabaseInfo(this.GetType().FullName),
            async (archive) =>
            {
                var entryVectorStore = archive.CreateEntry(DatabaseFile.vectorStoreFilename);
                using (var entryStream = entryVectorStore.Open())
                {
                    streamVectorStore.Position = 0;
                    await streamVectorStore.CopyToAsync(entryStream);
                    await entryStream.FlushAsync();
                }
            }
        );
        await stream.FlushAsync();
    }

    [Obsolete("Use SerializeToBinaryStream Instead")]
    public virtual void SerializeToJsonStream(Stream stream)
    {
        SerializeToBinaryStream(stream);
    }

    public virtual void SerializeToBinaryStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        SerializeToBinaryStreamAsync(stream).Wait();
    }

    [Obsolete("Use DeserializeFromBinaryStreamAsync Instead")]
    public virtual async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        await DeserializeFromBinaryStreamAsync(stream);
    }

    public virtual async Task DeserializeFromBinaryStreamAsync(Stream stream)
    {
        await DatabaseFile.LoadDatabaseFromZipArchiveAsync(
            stream,
            this.GetType().FullName,
            async (archive) =>
            {
                await DatabaseFile.LoadVectorStoreAsync(archive, VectorStore);

                // Re-initialize the IdGenerator with the max Id value from the VectorStore if it supports sequential numeric IDs
                if (_idGenerator is ISequentialIdGenerator<TId> seqIdGen)
                {
                    // Re-seed the sequence only if there are existing IDs
                    var ids = VectorStore.GetIds();
                    if (ids.Any())
                    {
                        seqIdGen.SetMostRecent(ids.Max()!);
                    }
                }
            }
        );
    }

    [Obsolete("Use DeserializeFromBinaryStream Instead")]
    public virtual void DeserializeFromJsonStream(Stream stream)
    {
        DeserializeFromBinaryStream(stream);
    }
    
    public virtual void DeserializeFromBinaryStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        DeserializeFromBinaryStreamAsync(stream).Wait();
    }

    public IEnumerator<IVectorTextDatabaseItem<TId, string, TMetadata>> GetEnumerator()
    {
        return VectorStore.Select(kvp => new VectorTextDatabaseItem<TId, string, TMetadata>(kvp.Key, kvp.Value.Text, kvp.Value.Metadata, kvp.Value.Vector)).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}