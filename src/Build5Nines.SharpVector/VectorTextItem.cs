namespace Build5Nines.SharpVector;

/// <summary>
/// An interface for storing a text with its metadata and vector data.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public interface IVectorTextItem<TDocument, TMetadata>
{
    TDocument Text { get; set; }
    TMetadata? Metadata { get; set; }
    float[] Vector { get; set; }
}

/// <summary>
/// An interface for storing a text with its metadata and vector.
/// </summary>
/// <typeparam name="TMetadata"></typeparam>
public interface IVectorTextItem<TMetadata> : IVectorTextItem<string, TMetadata>
{ }

/// <summary>
/// A class for storing a text with its metadata and vector.
/// </summary>
/// <typeparam name="TDocument"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class VectorTextItem<TDocument, TMetadata> : IVectorTextItem<TDocument, TMetadata>
{
    public VectorTextItem(TDocument text, TMetadata? metadata, float[] vector)
    {
        Text = text;
        Metadata = metadata;
        Vector = vector;
    }
    
    public TDocument Text { get; set; }
    public TMetadata? Metadata { get; set; }
    public float[] Vector { get; set; }
}

/// <summary>
/// A class for storing a text with its metadata and vector data.
/// </summary>
/// <typeparam name="TMetadata"></typeparam>
public class VectorTextItem<TMetadata> : VectorTextItem<string, TMetadata>, IVectorTextItem<TMetadata>
{
    public VectorTextItem(string text, TMetadata? metadata, float[] vector)
        : base(text, metadata, vector)
    { }
}