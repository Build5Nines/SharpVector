namespace Build5Nines.SharpVector.Vocabulary;


public interface IVocabularyStore<TKey, TValue>
    where TKey : notnull
{
    void Update(IEnumerable<TKey> tokens);
    Task UpdateAsync(IEnumerable<TKey> tokens);
    
    TValue Count { get; }
    bool TryGetValue(TKey token, out int index);
}