using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Data.Id;
using Build5Nines.SharpVector.Data.Vocabulary;

namespace Build5Nines.SharpVector;

public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVocabularyStore, TIdGenerator> : IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVocabularyStore : IVocabularyStore<string, int>
    where TIdGenerator : IIdGenerator<TId>, new()
{
    private Dictionary<TId, VectorTextItem<TMetadata>> _database;
    private TIdGenerator _idGenerator;

    protected TVocabularyStore VocabularyStore { get; private set; }

    public MemoryVectorDatabaseBase(TVocabularyStore vocabularyStore)
    {
        VocabularyStore = vocabularyStore;
        _database = new Dictionary<TId, VectorTextItem<TMetadata>>();
        _idGenerator = new TIdGenerator();
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public TId AddText(string text, TMetadata metadata)
    {
        var tokens = TokenizeAndPreprocess(text);
        VocabularyStore.Update(tokens);
        float[] vector = GenerateVectorFromTokens(tokens);
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
            var tokens = TokenizeAndPreprocess(text);
            VocabularyStore.Update(tokens);
            float[] vector = GenerateVectorFromTokens(tokens);
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
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public IVectorTextResult<TMetadata> Search(string queryText, int n = 5)
    {
        var similarityMatches = FindMostSimilarTextsIds(queryText, n);

        // Retrieve the actual texts for context
        var texts = new List<IVectorTextResultItem<TMetadata>>();
        foreach (var match in similarityMatches)
        {
            var item = GetText(match.Id);
            texts.Add(new VectorTextResultItem<TMetadata>(item, match.Similarity));
        }

        return new VectorTextResult<TMetadata>(texts.ToArray());
    }

    #region Private Methods

    /// <summary>
    /// Finds the top N most similar texts ids to the given text
    /// </summary>
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IEnumerable<VectorSimilarity<TId>> FindMostSimilarTextsIds(string queryText, int n = 5)
    {
        var queryTokens = TokenizeAndPreprocess(queryText);
        float[] queryVector = GenerateVectorFromTokens(queryTokens);

        // Method to get the maximum vector length in the database
        int desiredLength = _idGenerator.GetTotalCountGenerated();

        if (_database.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var similarities = new List<VectorSimilarity<TId>>();

        foreach (var kvp in _database)
        {
            float similarity = CalculateCosineSimilarity(NormalizeVector(queryVector, desiredLength), NormalizeVector(kvp.Value.Vector, desiredLength));
            similarities.Add(new VectorSimilarity<TId>(kvp.Key, similarity));
        }

        similarities = similarities.OrderByDescending(s => s.Similarity).Take(n).ToList();

        return similarities;
    }

    /// <summary>
    /// Method to normalize vectors to a specific length by padding or truncating
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static float[] NormalizeVector(float[] vector, int length)
    {
        float[] normalizedVector = new float[length];
        Array.Copy(vector, normalizedVector, Math.Min(vector.Length, length));
        return normalizedVector;
    }


    private static List<string> TokenizeAndPreprocess(string text)
    {
        text = text.ToLower();
        text = Regex.Replace(text, @"[^\w\s]", "");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text.Split(' ').ToList();
    }

    private float[] GenerateVectorFromTokens(List<string> tokens)
    {
        var vector = new float[VocabularyStore.Count];

        foreach (var token in tokens)
        {
            if (VocabularyStore.TryGetValue(token, out var index))
            {
                vector[index]++;
            }
        }

        return vector;
    }


    /// <summary>
    /// Calculates the cosine similarity between two vectors
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must be of the same length.");
        }

        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }

    #endregion
}