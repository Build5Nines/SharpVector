namespace Build5Nines.SharpVector.Vectorization;

using Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// An interface for classes that vectorizes a collection of tokens
/// </summary>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
public interface IVectorizer<TVocabularyKey, TVocabularyValue>
    where TVocabularyKey : notnull
    where TVocabularyValue : notnull
{
    /// <summary>
    /// Generates vectors from tokens using the vocabulary.
    /// </summary>
    /// <param name="vocabularyStore">The vocabulary store to use for vectorization</param>
    /// <param name="tokens">The tokens to generate a vector from</param>
    /// <returns></returns>
    float[] GenerateVectorFromTokens(IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore, IEnumerable<TVocabularyKey> tokens);

    /// <summary>
    /// Method to normalize vectors to a specific length by padding or truncating
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    float[] NormalizeVector(float[] vector, int length);
}