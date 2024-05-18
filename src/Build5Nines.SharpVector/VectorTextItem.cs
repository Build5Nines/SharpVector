public class VectorTextItem<TMetadata>
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