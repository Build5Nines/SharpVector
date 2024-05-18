namespace Build5Nines.SharpVector;

public interface IVectorTextItem<TMetadata>
{
    string Text { get; set; }
    TMetadata Metadata { get; set; }
    float[] Vector { get; set; }
}

public class VectorTextItem<TMetadata> : IVectorTextItem<TMetadata>
{
    public VectorTextItem(string text, TMetadata metadata, float[] vector)
    {
        Text = text;
        Metadata = metadata;
        Vector = vector;
    }
    
    public string Text { get; set; }
    public TMetadata Metadata { get; set; }
    public float[] Vector { get; set; }
}