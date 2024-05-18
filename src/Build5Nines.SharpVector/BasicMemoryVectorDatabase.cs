namespace Build5Nines.SharpVector;

/// <summary>
/// A basic implementation of an vector database that uses an in-memory dictionary to store vectors, with integer keys and string metadata values.
/// </summary>
public class BasicMemoryVectorDatabase : MemoryVectorDatabase<int, string>
{ }