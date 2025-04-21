using System.Reflection.Metadata;

namespace Build5Nines.SharpVector;

/// <summary>
/// An interface for a vector database
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TDocument"></typeparam>
public interface IVectorDatabase<TId, TMetadata, TDocument>
    : IEnumerable<IVectorTextDatabaseItem<TId, TDocument, TMetadata>>
    where TId : notnull
{
    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    TId AddText(TDocument text, TMetadata? metadata = default(TMetadata));

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    Task<TId> AddTextAsync(TDocument text, TMetadata? metadata = default(TMetadata));
    
    /// <summary>
    /// Get all the Ids for each text the database.
    /// </summary>
    /// <returns></returns>
    IEnumerable<TId> GetIds();

    /// <summary>
    /// Retrieves a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    IVectorTextItem<TDocument, TMetadata> GetText(TId id);

    /// <summary>
    /// Deletes a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    IVectorTextItem<TDocument, TMetadata> DeleteText(TId id);

    /// <summary>
    /// Updates a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    void UpdateText(TId id, TDocument text);

    /// <summary>
    /// Updates the Metadata of a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="metadata"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    void UpdateTextMetadata(TId id, TMetadata metadata);

    /// <summary>
    /// Updates a Text by its ID with new text and metadata values
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <param name="metadata"></param>
    void UpdateTextAndMetadata(TId id, TDocument text, TMetadata metadata);

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <returns></returns>
    IVectorTextResult<TId, TDocument, TMetadata> Search(TDocument queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null);

    /// <summary>
    /// Performs an asynchronous search vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <returns></returns>
    Task<IVectorTextResult<TId, TDocument, TMetadata>> SearchAsync(TDocument queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null);


    [Obsolete("Use SerializeToBinaryStreamAsync Instead")]
    Task SerializeToJsonStreamAsync(Stream stream);

    [Obsolete("Use SerializeToBinaryStream Instead")]
    void SerializeToJsonStream(Stream stream);

    [Obsolete("Use DeserializeToBinaryStreamAsync Instead")]
    Task DeserializeFromJsonStreamAsync(Stream stream);

    [Obsolete("Use DeserializeToBinaryStream Instead")]
    void DeserializeFromJsonStream(Stream stream);


    
    /// <summary>
    /// Serializes the Database to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task SerializeToBinaryStreamAsync(Stream stream);

    /// <summary>
    /// Serializes the Database to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    void SerializeToBinaryStream(Stream stream);

    /// <summary>
    /// Deserializes the Database from a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task DeserializeFromBinaryStreamAsync(Stream stream);

    /// <summary>
    /// Deserializes the Database from a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    void DeserializeFromBinaryStream(Stream stream);
}

public interface IVectorDatabase<TId, TMetadata>
 : IVectorDatabase<TId, TMetadata, string>
 where TId : notnull
{ }