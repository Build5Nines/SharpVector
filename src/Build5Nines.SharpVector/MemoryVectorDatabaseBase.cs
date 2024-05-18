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

    #region Private Methods


    private IEnumerable<VectorTextResultItem<TMetadata>> CalculateSimilarities(string queryText, float? threshold = null)
    {
        var queryTokens = TokenizeAndPreprocess(queryText);
        float[] queryVector = GenerateVectorFromTokens(queryTokens);

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
            float similarity = CalculateCosineSimilarity(NormalizeVector(queryVector, desiredLength), NormalizeVector(item.Vector, desiredLength));

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
        
        // Normalize the vector
        float magnitude = (float)Math.Sqrt(normalizedVector.Sum(v => v * v));
        if (magnitude > 0)
        {
            for (int i = 0; i < normalizedVector.Length; i++)
            {
                normalizedVector[i] /= magnitude;
            }
        }
        // else
        // {
        //     // If magnitude is zero, return the vector as it is
        //     // or handle it as per your requirement
        //     // For example, you can use a small value to avoid division by zero
        //     for (int i = 0; i < normalizedVector.Length; i++)
        //     {
        //         //normalizedVector[i] = 0; // or 
        //         normalizedVector[i] = 1e-10f;
        //     }
        // }
        
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