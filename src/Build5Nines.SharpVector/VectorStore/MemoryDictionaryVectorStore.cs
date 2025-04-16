using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Build5Nines.SharpVector.VectorStore;

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class MemoryDictionaryVectorStore<TId, TMetadata, TDocument> : IVectorStore<TId, TMetadata, TDocument>
    where TId : notnull
{
    private ConcurrentDictionary<TId, VectorTextItem<TDocument, TMetadata>> _database;

    /// <summary>
    /// The number of items in the database
    /// </summary>
    public int Count => _database.Count;

    public MemoryDictionaryVectorStore() {
        _database = new ConcurrentDictionary<TId, VectorTextItem<TDocument, TMetadata>>();
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public void Set(TId id, VectorTextItem<TDocument, TMetadata> item)
    {
        _database.AddOrUpdate(id, item, (key, oldValue) => item);
    }

    /// <summary>
    /// Gets all the Ids for every text.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TId> GetIds()
    {
        return _database.Keys;
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task SetAsync(TId id, VectorTextItem<TDocument, TMetadata> item)
    {
        await Task.Run(() => Set(id, item));
    }

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public VectorTextItem<TDocument, TMetadata> Get(TId id)
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
    /// <returns>The removed text item</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public VectorTextItem<TDocument, TMetadata> Delete(TId id)
    {
        if (_database.ContainsKey(id))
        {
            VectorTextItem<TDocument, TMetadata>? itemRemoved;
            _database.Remove(id, out itemRemoved);
#pragma warning disable CS8603 // Possible null reference return.
            return itemRemoved;
#pragma warning restore CS8603 // Possible null reference return.
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




    public IEnumerator<KeyValuePair<TId, VectorTextItem<TDocument, TMetadata>>> GetEnumerator()
    {
        return _database.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _database.GetEnumerator();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerator<KeyValuePair<TId, VectorTextItem<TDocument, TMetadata>>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var item in _database)
        {
            yield return item;
        }
    }

    public virtual async Task SerializeToJsonStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        await JsonSerializer.SerializeAsync<ConcurrentDictionary<TId, VectorTextItem<TDocument, TMetadata>>>(stream, _database);
    }

    public virtual async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        this._database = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<TId, VectorTextItem<TDocument, TMetadata>>>(stream) ?? new ConcurrentDictionary<TId, VectorTextItem<TDocument, TMetadata>>();
    }
}

/// <summary>
/// A thread safe simple in-memory database for storing and querying vectorized text items.
/// This is a simplified version of the MemoryDictionaryVectorStore class that uses string as the Document type.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class MemoryDictionaryVectorStore<TId, TMetadata>
    : MemoryDictionaryVectorStore<TId, TMetadata, string>
    where TId : notnull
{ }