namespace Build5Nines.SharpVector;

public interface IVectorTextDatabaseItem<TId, TDocument, TMetadata>
{
    TId Id { get; }
    TDocument Text { get; }
    TMetadata? Metadata { get; }
    float[] Vector { get; }
}

public class VectorTextDatabaseItem<TId, TDocument, TMetadata>
    : IVectorTextDatabaseItem<TId, TDocument, TMetadata>
{
    public VectorTextDatabaseItem(TId id, TDocument text, TMetadata? metadata, float[] vector)
    {
        Id = id;
        Text = text;
        Metadata = metadata;
        Vector = vector;
    }

    public TId Id { get; private set; }
    public TDocument Text { get; private set; }
    public TMetadata? Metadata { get; private set; }
    public float[] Vector { get; private set; }
}
