
using Build5Nines.SharpVector.Vocabulary;

namespace Build5Nines.SharpVector.VectorStore;

/// <summary>
/// Interface for a vector store with a vocabulary.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVocabularyStore"></typeparam>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
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