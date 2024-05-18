using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Build5Nines.SharpVector;

public class MemoryVectorDatabase<TMetadata> : IVectorDatabase<TMetadata>
{
    private Dictionary<int, VectorTextItem<TMetadata>> _database;
    private int _currentId;
    private Dictionary<string, int> _vocabulary;

    public MemoryVectorDatabase()
    {
        _database = new Dictionary<int, VectorTextItem<TMetadata>>();
        _currentId = 0;
        _vocabulary = new Dictionary<string, int>();
    }

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public int AddText(string text, TMetadata metadata)
    {
        var tokens = TokenizeAndPreprocess(text);
        UpdateVocabulary(tokens);
        float[] vector = GenerateVectorFromTokens(tokens);
        int id = _currentId++;
        _database[id] = new VectorTextItem<TMetadata>(text, metadata, vector);
        return id;
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public VectorTextItem<TMetadata> GetText(int id)
    {
        if (_database.TryGetValue(id, out var entry))
        {
            return entry;
        }
        throw new KeyNotFoundException($"Text with ID {id} not found.");
    }

    /// <summary>
    /// Updates a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateText(int id, string text)
    {
        if (_database.ContainsKey(id))
        {
            var tokens = TokenizeAndPreprocess(text);
            UpdateVocabulary(tokens);
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
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void DeleteText(int id)
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
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public IVectorTextResult<TMetadata> Search(string queryText, int n = 5)
    {
        var topSimilarIds = FindMostSimilarTextsIds(queryText, n);

        // Retrieve the actual texts for context
        var texts = new List<VectorTextResultItem<TMetadata>>();
        foreach (var id in topSimilarIds)
        {
            var item = GetText(id);
            texts.Add(new VectorTextResultItem<TMetadata>(item));
        }

        return new VectorTextResult<TMetadata>(texts.ToArray());
    }

    /// <summary>
    /// Finds the top N most similar texts ids to the given text
    /// </summary>
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private int[] FindMostSimilarTextsIds(string queryText, int n = 5)
    {
        var queryTokens = TokenizeAndPreprocess(queryText);
        float[] queryVector = GenerateVectorFromTokens(queryTokens);
        int desiredLength = GetMaxVectorLength();

        if (_database.Count == 0)
        {
            throw new InvalidOperationException("The database is empty.");
        }

        var similarities = new List<(int Id, float Similarity)>();

        foreach (var kvp in _database)
        {
            float similarity = CalculateCosineSimilarity(NormalizeVector(queryVector, desiredLength), NormalizeVector(kvp.Value.Vector, desiredLength));
            similarities.Add((kvp.Key, similarity));
        }

        similarities = similarities.OrderByDescending(s => s.Similarity).Take(n).ToList();

        return similarities.Select(s => s.Id).ToArray();
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

    /// <summary>
    /// Method to get the maximum vector length in the database
    /// </summary>
    /// <returns></returns>
    private int GetMaxVectorLength()
    {
        return _vocabulary.Count;
    }

    private static List<string> TokenizeAndPreprocess(string text)
    {
        text = text.ToLower();
        text = Regex.Replace(text, @"[^\w\s]", "");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text.Split(' ').ToList();
    }

     private void UpdateVocabulary(List<string> tokens)
    {
        foreach (var token in tokens)
        {
            if (!_vocabulary.ContainsKey(token))
            {
                _vocabulary[token] = _vocabulary.Count;
            }
        }
    }

    private float[] GenerateVectorFromTokens(List<string> tokens)
    {
        var vector = new float[_vocabulary.Count];

        foreach (var token in tokens)
        {
            if (_vocabulary.TryGetValue(token, out var index))
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
}
