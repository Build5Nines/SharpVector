using System.Collections.Concurrent;
using System.Text.Json;

namespace Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DictionaryVocabularyStore<TKey> : IVocabularyStore<TKey, int>
    where TKey : notnull
{
    private ConcurrentDictionary<TKey, int> _vocabulary;

    public DictionaryVocabularyStore()
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

    public async Task SerializeToJsonStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        await JsonSerializer.SerializeAsync<ConcurrentDictionary<TKey, int>>(stream, _vocabulary);
    }

    public async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        this._vocabulary = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<TKey, int>>(stream) ?? new ConcurrentDictionary<TKey, int>();
    }
}