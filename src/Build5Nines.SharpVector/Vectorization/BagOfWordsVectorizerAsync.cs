namespace Build5Nines.SharpVector.Vectorization;

using Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// A class that vectorizes a collection of tokens
/// </summary>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
public class BagOfWordsVectorizerAsync<TVocabularyKey, TVocabularyValue> : IVectorizerAsync<TVocabularyKey, TVocabularyValue>
    where TVocabularyKey : notnull
    where TVocabularyValue : notnull
{
    public async Task<float[]> GenerateVectorFromTokensAsync(IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore, IEnumerable<TVocabularyKey> tokens)
    {
        return await Task.Run(() => GenerateVectorFromTokens(vocabularyStore, tokens));
    }

    /// <summary>
    /// Generates vectors from tokens using the vocabulary.
    /// </summary>
    /// <param name="vocabularyStore">The vocabulary store to use for vectorization</param>
    /// <param name="tokens">The tokens to generate a vector from</param>
    /// <returns></returns>
    public float[] GenerateVectorFromTokens(IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore, IEnumerable<TVocabularyKey> tokens)
    {
        dynamic count = vocabularyStore.Count;
        var vector = new float[count];

        foreach (var token in tokens)
        {
            if (vocabularyStore.TryGetValue(token, out var index))
            {
                vector[index]++;
            }
        }

        return vector;
    }

    /// <summary>
    /// Method to normalize vectors to a specific length by padding or truncating
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public float[] NormalizeVector(float[] vector, int length)
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
}