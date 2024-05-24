using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using System.Collections.Concurrent;

namespace Build5Nines.SharpVector;

public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TVocabularyStore, TIdGenerator, TTextPreprocessor, TVectorizer, TVectorComparer>
    : IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVectorStore : IVectorStore<TId, TMetadata>
    where TVocabularyStore : IVocabularyStore<string, int>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TTextPreprocessor : ITextPreprocessor, new()
    where TVectorizer : IVectorizer<string, int>, new()
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

    /// <summary>
    /// The Vocabulary Store used to store the vocabulary of the database
    /// </summary>
    protected TVocabularyStore VocabularyStore { get; private set; }

    public MemoryVectorDatabaseBase(TVectorStore vectorStore, TVocabularyStore vocabularyStore)
    {
        VectorStore = vectorStore;
        VocabularyStore = vocabularyStore;
        _idGenerator = new TIdGenerator();
        _textPreprocessor = new TTextPreprocessor();
        _vectorizer = new TVectorizer();
        _vectorComparer = new TVectorComparer();
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public TId AddText(string text, TMetadata? metadata = default(TMetadata))
    {
        // Perform preprocessing asynchronously
        var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
        
        // Update the vocabulary store asynchronously
        VocabularyStore.Update(tokens);
        
        // Generate the vector asynchronously
        float[] vector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, tokens);
        
        // Generate the ID and store the vector text item asynchronously
        TId id = _idGenerator.NewId();
        VectorStore.Set(id, new VectorTextItem<TMetadata>(text, metadata, vector));
        
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
    public void DeleteText(TId id)
    {
        VectorStore.Delete(id);
    }

    /// <summary>
    /// Updates a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateText(TId id, string text)
    {
        if (VectorStore.ContainsKey(id))
        {
            var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
            VocabularyStore.Update(tokens);
            float[] vector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, tokens);
            var metadata = VectorStore.Get(id).Metadata;
            VectorStore.Set(id, new VectorTextItem<TMetadata>(text, metadata, vector));
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
    public void UpdateTextAndMetadata(TId id, string text, TMetadata metadata)
    {
        if (VectorStore.ContainsKey(id))
        {
            var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
            VocabularyStore.Update(tokens);
            float[] vector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, tokens);
            VectorStore.Set(id, new VectorTextItem<TMetadata>(text, metadata, vector));
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
    public IVectorTextResult<TMetadata> Search(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
    {
        var similarities = CalculateVectorComparison(queryText, threshold);

        similarities = _vectorComparer.Sort(similarities);

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

    private IEnumerable<VectorTextResultItem<TMetadata>> CalculateVectorComparison(string queryText, float? threshold = null)
    {
        var queryTokens = _textPreprocessor.TokenizeAndPreprocess(queryText);
        float[] queryVector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, queryTokens);

        // Method to get the maximum vector length in the database
        int desiredLength = VocabularyStore.Count;

        if (VectorStore.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var similarities = new ConcurrentBag<VectorTextResultItem<TMetadata>>();
        Parallel.ForEach(VectorStore, kvp =>
        {
            var item = kvp.Value;
            float vectorComparisonValue = _vectorComparer.Calculate(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));

            if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
            {
                similarities.Add(new VectorTextResultItem<TMetadata>(item, vectorComparisonValue));
            }
        });

        // var similarities = new List<VectorTextResultItem<TMetadata>>();
        // foreach (var kvp in VectorStore)
        // {
        //     var item = kvp.Value;
        //     float vectorComparisonValue = _vectorComparer.Calculate(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));

        //     if (_vectorComparer.IsWithinThreshold(threshold, vectorComparisonValue))
        //     {
        //         similarities.Add(new VectorTextResultItem<TMetadata>(item, vectorComparisonValue));
        //     }
        // }

        return similarities;
    }
}