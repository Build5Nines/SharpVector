namespace Build5Nines.SharpVector.Data.Vocabulary;


public interface IVocabularyStore<TKey, TValue>
    where TKey : notnull
{
    void Update(List<TKey> tokens);
    TValue Count { get; }
    bool TryGetValue(TKey token, out int index);
}