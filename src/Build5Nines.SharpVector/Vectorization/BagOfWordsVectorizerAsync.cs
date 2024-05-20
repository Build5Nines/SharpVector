namespace Build5Nines.SharpVector.Vectorization;

using Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// A class that vectorizes a collection of tokens
/// </summary>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
public class BagOfWordsVectorizerAsync<TVocabularyKey, TVocabularyValue> : BagOfWordsVectorizer<TVocabularyKey, TVocabularyValue>, IVectorizerAsync<TVocabularyKey, TVocabularyValue>
    where TVocabularyKey : notnull
    where TVocabularyValue : notnull
{
    public async Task<float[]> GenerateVectorFromTokensAsync(IVocabularyStore<TVocabularyKey, TVocabularyValue> vocabularyStore, IEnumerable<TVocabularyKey> tokens)
    {
        return await Task.Run(() => GenerateVectorFromTokens(vocabularyStore, tokens));
    }
}