namespace Build5Nines.SharpVector;

public interface IVectorTextItem<TDocument, TMetadata>
{
    TDocument Text { get; set; }
    TMetadata? Metadata { get; set; }
    float[] Vector { get; set; }
}

public interface IVectorTextItem<TMetadata> : IVectorTextItem<string, TMetadata>
{ }

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

public class VectorTextItem<TMetadata> : VectorTextItem<string, TMetadata>, IVectorTextItem<TMetadata>
{
    public VectorTextItem(string text, TMetadata? metadata, float[] vector)
        : base(text, metadata, vector)
    { }
}