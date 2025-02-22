namespace Build5Nines.SharpVector.Vocabulary;


public interface IVocabularyStore<TKey, TValue>
    where TKey : notnull
{
    /// <summary>
    /// Updates the vocabulary store
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    void Update(IEnumerable<TKey> tokens);

    /// <summary>
    /// Updates the vocabulary store asynchronously
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    Task UpdateAsync(IEnumerable<TKey> tokens);
    
    /// <summary>
    /// The number of items in the vocabulary store
    /// </summary>
    TValue Count { get; }

    /// <summary>
    /// Retrieves the index of a token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    bool TryGetValue(TKey token, out int index);

    /// <summary>
    /// Serializes the Vocabulary Store to a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task SerializeToJsonStreamAsync(Stream stream);

    /// <summary>
    /// Deserializes the Vocabulary Store from a JSON stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Task DeserializeFromJsonStreamAsync(Stream stream);
}