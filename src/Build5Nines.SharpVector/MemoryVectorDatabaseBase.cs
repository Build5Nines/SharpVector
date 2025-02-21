using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using System.Collections.Concurrent;

namespace Build5Nines.SharpVector;

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
    private TIdGenerator _idGenerator;

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
    /// <returns></returns>
    public IVectorTextResult<TVocabularyKey, TMetadata> Search(TVocabularyKey queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
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
    public async Task<IVectorTextResult<TVocabularyKey, TMetadata>> SearchAsync(TVocabularyKey queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
    {
        var similarities = await CalculateVectorComparisonAsync(queryText, threshold);

        similarities = await _vectorComparer.SortAsync(similarities);

        var totalCountFoundInSearch = similarities.Count();

        IEnumerable<VectorTextResultItem<TVocabularyKey, TMetadata>> resultsToReturn;
        if (pageCount != null && pageCount >= 0 && pageIndex >= 0) {
            resultsToReturn = similarities.Skip(pageIndex * pageCount.Value).Take(pageCount.Value);
        } else {
            // no paging specified, return all results
            resultsToReturn = similarities;
        }

        return new VectorTextResult<TVocabularyKey, TMetadata>(totalCountFoundInSearch, pageIndex, pageCount.HasValue ? pageCount.Value : 1, resultsToReturn);
    }

    private async Task<IEnumerable<VectorTextResultItem<TVocabularyKey, TMetadata>>> CalculateVectorComparisonAsync(TVocabularyKey queryText, float? threshold = null)
    {
        var queryTokens = _textPreprocessor.TokenizeAndPreprocess(queryText);
        float[] queryVector = _vectorizer.GenerateVectorFromTokens(VectorStore.VocabularyStore, queryTokens);

        // Method to get the maximum vector length in the database
        var desiredLength = VectorStore.VocabularyStore.Count;

        if (VectorStore.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var results = new ConcurrentBag<VectorTextResultItem<TVocabularyKey, TMetadata>>();
        await foreach (var kvp in VectorStore)
        {
            var item = kvp.Value;
            float vectorComparisonValue = await _vectorComparer.CalculateAsync(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));

            if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
            {
                results.Add(new VectorTextResultItem<TVocabularyKey, TMetadata>(item, vectorComparisonValue));
            }
        }
        return results;
    }
}