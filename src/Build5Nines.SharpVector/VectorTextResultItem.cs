namespace Build5Nines.SharpVector;

public interface IVectorTextResultItem<TMetadata>
{
    string Text{ get; }
    TMetadata Metadata { get; }

    float Similarity { get; }
}

public class VectorTextResultItem<TMetadata> : IVectorTextResultItem<TMetadata>
{
    private IVectorTextItem<TMetadata> _item;
    public VectorTextResultItem(IVectorTextItem<TMetadata> item, float similarity){
        _item = item;
        Similarity = similarity;
    }
    
    public string Text { get => _item.Text; }
    public TMetadata Metadata { get => _item.Metadata; }
    public float Similarity { get; private set; }
}