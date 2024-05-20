namespace Build5Nines.SharpVector;

public interface IVectorTextResultItem<TMetadata>
{
    string Text{ get; }
    TMetadata? Metadata { get; }

    float VectorComparison { get; }
}

public class VectorTextResultItem<TMetadata> : IVectorTextResultItem<TMetadata>
{
    private IVectorTextItem<TMetadata> _item;
    public VectorTextResultItem(IVectorTextItem<TMetadata> item, float vectorComparison)
    {
        _item = item;
        VectorComparison = vectorComparison;
    }
    
    public string Text { get => _item.Text; }
    public TMetadata? Metadata { get => _item.Metadata; }
    public float VectorComparison { get; private set; }
}