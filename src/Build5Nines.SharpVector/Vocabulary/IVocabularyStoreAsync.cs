namespace Build5Nines.SharpVector.Vocabulary;


public interface IVocabularyStoreAsync<TKey, TValue> : IVocabularyStore<TKey, TValue>
    where TKey : notnull
{
    Task UpdateAsync(IEnumerable<TKey> tokens);
}