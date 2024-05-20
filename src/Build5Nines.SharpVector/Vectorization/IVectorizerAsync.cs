namespace Build5Nines.SharpVector.Vectorization;

using Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// An interface for classes that vectorizes a collection of tokens
/// </summary>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
public interface IVectorizerAsync<TVocabularyKey, TVocabularyValue> : IVectorizer<TVocabularyKey, TVocabularyValue>
    where TVocabularyKey : notnull
    where TVocabularyValue : notnull
{
    /// <summary>
    /// Generates vectors from tokens using the vocabulary asynchronously.
    /// </summary>
    /// <param name="vocabularyStore"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    Task<float[]> GenerateVectorFromTokensAsync(IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore, IEnumerable<TVocabularyKey> tokens);
}