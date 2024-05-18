namespace Build5Nines.SharpVector.Data.Vocabulary;

public class DictionaryVocabularyStore<TKey> : IVocabularyStore<TKey, int>
    where TKey : notnull
{
    private Dictionary<TKey, int> _vocabulary;

    public DictionaryVocabularyStore()
    {
        _vocabulary = new Dictionary<TKey, int>();
    }

    public void Update(List<TKey> tokens)
    {
        foreach (var token in tokens)
        {
            if (!_vocabulary.ContainsKey(token))
            {
                _vocabulary[token] = Count;
            }
        }
    }
    
    public int Count { get => _vocabulary.Count; }

    public bool TryGetValue(TKey token, out int index)
    {
        return _vocabulary.TryGetValue(token, out index);
    }
}