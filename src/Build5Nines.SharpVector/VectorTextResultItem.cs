using System.Collections.Immutable;
using Build5Nines.SharpVector.Id;

namespace Build5Nines.SharpVector;

/// <summary>
/// Represents a result item from a semantic search on a vector database.
/// </summary>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public interface IVectorTextResultItem<TDocument, TMetadata>
{
    /// <summary>
    /// The string of text that was vectorized.
    /// </summary>
    TDocument Text{ get; }

    /// <summary>
    /// The metadata associated with the text.
    /// </summary>
    TMetadata? Metadata { get; }

    /// <summary>
    /// The vector similarity score between the query and the text. (This is deprecated, use 'Similarity' instead)
    /// </summary>
    [Obsolete("Use 'Similarity' instead")]
    float VectorComparison { get; }

    /// <summary>
    /// The vector similarity score between the query and the text.
    /// </summary>
    float Similarity { get; }
}

/// <summary>
/// Represents a result item from a semantic search on a vector database.
/// </summary>
/// <typeparam name="TId">The type of the ID.</typeparam>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public interface IVectorTextResultItem<TId, TDocument, TMetadata>
    : IVectorTextResultItem<TDocument, TMetadata>
{
    TId Id { get; }
}

/// <summary>
/// Represents a result item from a semantic search on a vector database.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public interface IVectorTextResultItem<TMetadata>
 : IVectorTextResultItem<string, TMetadata>, IVectorTextResultItem<int, string, TMetadata>
{ }

/// <summary>
/// Represents a result item from a semantic search on a vector database.
/// </summary>
/// <typeparam name="TId">The type of the ID.</typeparam>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public class VectorTextResultItem<TId, TDocument, TMetadata>
    : IVectorTextResultItem<TDocument, TMetadata>, IVectorTextResultItem<TId, TDocument, TMetadata>
{
    private IVectorTextItem<TDocument, TMetadata> _item;
    private TId _id;

    public VectorTextResultItem(TId id, IVectorTextItem<TDocument, TMetadata> item, float similarity)
    {
        _id = id;
        _item = item;
        Similarity = similarity;
    }
    
    /// <summary>
    /// The string of text that was vectorized.
    /// </summary>
    public TDocument Text { get => _item.Text; }

    /// <summary>
    /// The metadata associated with the text.
    /// </summary>
    public TMetadata? Metadata { get => _item.Metadata; }
    public TId Id { get => _id; }

    /// <summary>
    /// The vector representation / embeddings of the text.
    /// </summary>
    public ImmutableArray<float> Vectors { get => ImmutableArray.Create(_item.Vector); }

    /// <summary>
    /// The vector similarity score between the query and the text.
    /// </summary>
    public float Similarity { get; private set; }

    /// <summary>
    /// The vector similarity score between the query and the text. (This is deprecated, use 'Similarity' instead)
    /// </summary>
    [Obsolete("Use 'Similarity' instead")]
    public float VectorComparison { get => Similarity; }
}

/// <summary>
/// Represents a result item from a semantic search on a vector database.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public class VectorTextResultItem<TMetadata>
 : VectorTextResultItem<int, string, TMetadata>, IVectorTextResultItem<TMetadata>
{
    public VectorTextResultItem(int id, IVectorTextItem<string, TMetadata> item, float vectorComparison)
        : base(id, item, vectorComparison)
    { }
}

