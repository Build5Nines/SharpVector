using System.Collections;

namespace Build5Nines.SharpVector.VectorStore;

public interface IVectorStoreAsync<TId, TMetadata> : IVectorStore<TId, TMetadata>, IAsyncEnumerable<KeyValuePair<TId, IVectorTextItem<TMetadata>>>
{
    /// <summary>
    /// Retrieves a text and metadata by its ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    Task SetAsync(TId id, IVectorTextItem<TMetadata> item);
}