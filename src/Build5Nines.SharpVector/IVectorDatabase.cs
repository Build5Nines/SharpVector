namespace Build5Nines.SharpVector;

public interface IVectorDatabase<TMetadata>
{
    /// <summary>
    /// Adds a new text with Metadata to the database and returns its ID
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    int AddText(string text, TMetadata metadata);

    /// <summary>
    /// Performs a vector search to find the top N most similar texts to the given text
    /// </summary>
    /// <param name="queryText"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    IVectorTextResult<TMetadata> Search(string queryText, int n = 5);
}
