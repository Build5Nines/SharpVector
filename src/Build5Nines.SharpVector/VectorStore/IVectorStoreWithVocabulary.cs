
using Build5Nines.SharpVector.Vocabulary;

namespace Build5Nines.SharpVector.VectorStore;

public interface IVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
 : IVectorStore<TId, TMetadata, TVocabularyKey>
    where TId : notnull
    where TVocabularyKey : notnull
    where TVocabularyStore : IVocabularyStore<TVocabularyKey, TVocabularyValue>
{
    /// <summary>
    /// The Vocabulary Store used to store the vocabulary of the database
    /// </summary>
    TVocabularyStore VocabularyStore { get; }
}