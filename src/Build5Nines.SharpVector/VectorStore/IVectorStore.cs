using System.Collections;
using System.Runtime.Serialization;

namespace Build5Nines.SharpVector.VectorStore;

public interface IVectorStore<TId, TMetadata, TDocument>
    : IEnumerable<KeyValuePair<TId, VectorTextItem<TDocument, TMetadata>>>,
    IReadOnlyCollection<KeyValuePair<TId, VectorTextItem<TDocument, TMetadata>>>,
    IEnumerable,
    IAsyncEnumerable<KeyValuePair<TId, VectorTextItem<TDocument, TMetadata>>>
{
    /// <summary>
    /// Retrieves a text and metadata by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    VectorTextItem<TDocument, TMetadata> Get(TId id);

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
    void Set(TId id, VectorTextItem<TDocument, TMetadata> item);

    /// <summary>
    /// Retrieves a text and metadata by its ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    Task SetAsync(TId id, VectorTextItem<TDocument, TMetadata> item);

    /// <summary>
    /// Deletes a text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The removed text item</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    VectorTextItem<TDocument, TMetadata> Delete(TId id);

    /// <summary>
    /// Checks if the database contains a key
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool ContainsKey(TId id);

    /// <summary>
    /// Serializes the Vector Store to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task SerializeToJsonStreamAsync(Stream stream);

    /// <summary>
    /// Deserializes the Vector Store from a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task DeserializeFromJsonStreamAsync(Stream stream);
}

public interface IVectorStore<TId, TMetadata> : IVectorStore<TId, TMetadata, string>
{ }
