using System.Collections.Concurrent;

namespace Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DictionaryVocabularyStoreAsync<TKey> : IVocabularyStoreAsync<TKey, int>
    where TKey : notnull
{
    private ConcurrentDictionary<TKey, int> _vocabulary;

    public DictionaryVocabularyStoreAsync()
    {
        _vocabulary = new ConcurrentDictionary<TKey, int>();
    }

    private object _lock = new object();

    public void Update(IEnumerable<TKey> tokens)
    {
        lock(_lock) {
            foreach (var token in tokens)
            {
                if (!_vocabulary.ContainsKey(token))
                {
                    _vocabulary[token] = Count;
                }
            }
        }
    }

    public async Task UpdateAsync(IEnumerable<TKey> tokens)
    {
        await Task.Run(() => Update(tokens));
    }
    
    public int Count { get => _vocabulary.Count; }

    public bool TryGetValue(TKey token, out int index)
    {
        return _vocabulary.TryGetValue(token, out index);
    }
}