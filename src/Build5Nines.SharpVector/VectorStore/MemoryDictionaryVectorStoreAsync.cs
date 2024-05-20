using System.Collections;
using System.Collections.Concurrent;

namespace Build5Nines.SharpVector.VectorStore;

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class MemoryDictionaryVectorStoreAsync<TId, TMetadata> : IVectorStoreAsync<TId, TMetadata>
    where TId : notnull
{
    private ConcurrentDictionary<TId, IVectorTextItem<TMetadata>> _database;

    /// <summary>
    /// The number of items in the database
    /// </summary>
    public int Count => _database.Count;

    public MemoryDictionaryVectorStoreAsync() {
        _database = new ConcurrentDictionary<TId, IVectorTextItem<TMetadata>>();
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public void Set(TId id, IVectorTextItem<TMetadata> item)
    {
        _database.AddOrUpdate(id, item, (key, oldValue) => item);
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task SetAsync(TId id, IVectorTextItem<TMetadata> item)
    {
        await Task.Run(() => Set(id, item));
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public IVectorTextItem<TMetadata> Get(TId id)
    {
        if (_database.TryGetValue(id, out var entry))
        {
            return entry;
        }
        throw new KeyNotFoundException($"Text with ID {id} not found.");
    }

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void Delete(TId id)
    {
        if (_database.ContainsKey(id))
        {
            IVectorTextItem<TMetadata> itemRemoved;
            _database.Remove(id, out itemRemoved);
        }
        else
        {
            throw new KeyNotFoundException($"Text with ID {id} not found.");
        }
    }

    /// <summary>
    /// Checks if the database contains a key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool ContainsKey(TId id) => _database.ContainsKey(id);




    public IEnumerator<KeyValuePair<TId, IVectorTextItem<TMetadata>>> GetEnumerator()
    {
        return _database.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _database.GetEnumerator();
    }
}