using Build5Nines.SharpVector.VectorEncoding;

namespace Build5Nines.SharpVector;

/// <summary>
/// A basic implementation of an vector database that uses an in-memory dictionary to store vectors, with integer keys and string metadata values.
/// </summary>
public class BasicMemoryVectorDatabase : MemoryVectorDatabase<string>
{
    public BasicMemoryVectorDatabase()
        : base()
    { }

    /// <summary>
    /// Create a database that compresses vectors with the supplied encoding
    /// before storing them.
    /// </summary>
    public BasicMemoryVectorDatabase(IVectorEncoding encoding)
        : base(encoding)
    { }
}
