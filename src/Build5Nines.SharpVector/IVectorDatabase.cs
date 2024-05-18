namespace Build5Nines.SharpVector;

public interface IVectorDatabase<TId, TMetadata>
{
    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    TId AddText(string text, TMetadata metadata);

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
    void DeleteText(TId id);

    /// <summary>
    /// Updates a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void UpdateText(TId id, string text);

    /// <summary>
    /// Updates the Metadata of a Text by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="metadata"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    void UpdateTextMetadata(TId id, TMetadata metadata);

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    IVectorTextResult<TMetadata> Search(string queryText, int n = 5);
}
