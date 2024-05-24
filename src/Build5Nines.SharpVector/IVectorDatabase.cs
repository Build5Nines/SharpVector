namespace Build5Nines.SharpVector;

public interface IVectorDatabase<TId, TMetadata>
    where TId : notnull
{
    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    TId AddText(string text, TMetadata? metadata = default(TMetadata));

    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    Task<TId> AddTextAsync(string text, TMetadata? metadata = default(TMetadata));
    
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
    IVectorTextItem<TMetadata> GetText(TId id);

    /// <summary>
    /// Deletes a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    IVectorTextItem<TMetadata> DeleteText(TId id);

    /// <summary>
    /// Updates a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    void UpdateText(TId id, string text);

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
    void UpdateTextAndMetadata(TId id, string text, TMetadata metadata);

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <returns></returns>
    IVectorTextResult<TMetadata> Search(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null);

    /// <summary>
    /// Performs an asynchronous search vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText">The query prompt to search by.</param>
    /// <param name="threshold">The similarity threshold to filter by.</param>
    /// <param name="pageIndex">The page index of the search results. Default is 0.</param>
    /// <param name="pageCount">The number of search results per page. Default is Null and returns all results.</param>
    /// <returns></returns>
    Task<IVectorTextResult<TMetadata>> SearchAsync(string queryText, float? threshold = null, int pageIndex = 0, int? pageCount = null);
}
