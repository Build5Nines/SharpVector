using System.Collections;

namespace Build5Nines.SharpVector.VectorStore;

public interface IVectorStore<TId, TMetadata, TDocument>
    : IEnumerable<KeyValuePair<TId, IVectorTextItem<TDocument, TMetadata>>>,
    IReadOnlyCollection<KeyValuePair<TId, IVectorTextItem<TDocument, TMetadata>>>,
    IEnumerable,
    IAsyncEnumerable<KeyValuePair<TId, IVectorTextItem<TDocument, TMetadata>>>
{
    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    IVectorTextItem<TDocument, TMetadata> Get(TId id);

    /// <summary>
    /// Gets all the Ids for every text.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TId> GetIds();

    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    void Set(TId id, IVectorTextItem<TDocument, TMetadata> item);

    /// <summary>
    /// Retrieves a text and metadata by its ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    Task SetAsync(TId id, IVectorTextItem<TDocument, TMetadata> item);

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The removed text item</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    IVectorTextItem<TDocument, TMetadata> Delete(TId id);

    /// <summary>
    /// Checks if the database contains a key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool ContainsKey(TId id);
}

public interface IVectorStore<TId, TMetadata> : IVectorStore<TId, TMetadata, string>
{ }
