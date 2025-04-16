
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items with a vocabulary.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVocabularyStore"></typeparam>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
public class MemoryDictionaryVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
    : MemoryDictionaryVectorStore<TId, TMetadata, TVocabularyKey>, IVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
    where TId : notnull
    where TVocabularyKey : notnull
    where TVocabularyStore : IVocabularyStore<TVocabularyKey, TVocabularyValue>
{
    public TVocabularyStore VocabularyStore { get; }

    public MemoryDictionaryVectorStoreWithVocabulary(TVocabularyStore vocabularyStore)
    {
        VocabularyStore = vocabularyStore;
    }
}