namespace Build5Nines.SharpVector;

public interface IVectorDatabaseAsync<TId, TMetadata> : IVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TMetadata : notnull
{
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
