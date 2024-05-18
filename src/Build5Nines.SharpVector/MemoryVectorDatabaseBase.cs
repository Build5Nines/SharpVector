using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.Similarity;

namespace Build5Nines.SharpVector;

public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVocabularyStore, TIdGenerator, TTextPreprocessor, TVectorizer, TVectorSimilarityCalculator> : IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVocabularyStore : IVocabularyStore<string, int>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TTextPreprocessor : ITextPreprocessor, new()
    where TVectorizer : IVectorizer<string, int>, new()
    where TVectorSimilarityCalculator : IVectorSimilarityCalculator, new()
{
    private Dictionary<TId, VectorTextItem<TMetadata>> _database;
    private TIdGenerator _idGenerator;

    private TTextPreprocessor _textPreprocessor;

    private TVectorizer _vectorizer;

    private TVectorSimilarityCalculator _vectorSimilarityCalculator;

    protected TVocabularyStore VocabularyStore { get; private set; }

    public MemoryVectorDatabaseBase(TVocabularyStore vocabularyStore)
    {
        VocabularyStore = vocabularyStore;
        _database = new Dictionary<TId, VectorTextItem<TMetadata>>();
        _idGenerator = new TIdGenerator();
        _textPreprocessor = new TTextPreprocessor();
        _vectorizer = new TVectorizer();
        _vectorSimilarityCalculator = new TVectorSimilarityCalculator();
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public TId AddText(string text, TMetadata metadata)
    {
        var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
        VocabularyStore.Update(tokens);
        float[] vector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, tokens);
        TId id = _idGenerator.NewId();
        _database[id] = new VectorTextItem<TMetadata>(text, metadata, vector);
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
        if (_database.TryGetValue(id, out var entry))
        {
            return entry;
        }
        throw new KeyNotFoundException($"Text with ID {id} not found.");
    }

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void DeleteText(TId id)
    {
        if (_database.ContainsKey(id))
        {
            _database.Remove(id);
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
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateText(TId id, string text)
    {
        if (_database.ContainsKey(id))
        {
            var tokens = _textPreprocessor.TokenizeAndPreprocess(text);
            VocabularyStore.Update(tokens);
            float[] vector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, tokens);
            var metadata = _database[id].Metadata;
            _database[id] = new VectorTextItem<TMetadata>(text, metadata, vector);
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
        if (_database.ContainsKey(id))
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
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="topN">The highest number of results to show.</param>
    /// <param name="threshold">The similarity threshold. Only return items greater or equal to the threshold. Null returns all.</param>
    /// <returns></returns>
    public IVectorTextResult<TMetadata> Search(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null)
    {
        var similarities = CalculateSimilarities(queryText, threshold).OrderByDescending(s => s.Similarity);

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

    private IEnumerable<VectorTextResultItem<TMetadata>> CalculateSimilarities(string queryText, float? threshold = null)
    {
        var queryTokens = _textPreprocessor.TokenizeAndPreprocess(queryText);
        float[] queryVector = _vectorizer.GenerateVectorFromTokens(VocabularyStore, queryTokens);

        // Method to get the maximum vector length in the database
        int desiredLength = VocabularyStore.Count;

        if (_database.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var similarities = new List<VectorTextResultItem<TMetadata>>();


        bool includeAll = threshold == null;
        float thresholdToCompare = threshold ?? (float)0.0f;
        bool includeSimilarity = true;
        bool thresholdIsEqual;
        foreach (var kvp in _database)
        {
            var item = kvp.Value;
            float similarity = _vectorSimilarityCalculator.CalculateVectorSimilarity(_vectorizer.NormalizeVector(queryVector, desiredLength), _vectorizer.NormalizeVector(item.Vector, desiredLength));

            if (!includeAll) {
                thresholdIsEqual = Math.Abs(similarity - thresholdToCompare) < 1e-6f; // epsilon;
                includeSimilarity = thresholdIsEqual || similarity >= thresholdToCompare;
            }

            if (includeAll || includeSimilarity) {
                similarities.Add(new VectorTextResultItem<TMetadata>(item, similarity));
            }
        }

        return similarities;
    }
}