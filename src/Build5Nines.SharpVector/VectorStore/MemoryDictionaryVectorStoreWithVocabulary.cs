
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

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