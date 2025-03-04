using System.Collections.Immutable;

namespace Build5Nines.SharpVector;

public interface IVectorTextResultItem<TDocument, TMetadata>
{
    TDocument Text{ get; }
    TMetadata? Metadata { get; }

    float VectorComparison { get; }
}

public interface IVectorTextResultItem<TMetadata>
 : IVectorTextResultItem<string, TMetadata>
{ }

public class VectorTextResultItem<TDocument, TMetadata> : IVectorTextResultItem<TDocument, TMetadata>
{
    private IVectorTextItem<TDocument, TMetadata> _item;
    public VectorTextResultItem(IVectorTextItem<TDocument, TMetadata> item, float vectorComparison)
    {
        _item = item;
        VectorComparison = vectorComparison;
    }
    
    public TDocument Text { get => _item.Text; }
    public TMetadata? Metadata { get => _item.Metadata; }

    public ImmutableArray<float> Vectors { get => ImmutableArray.Create(_item.Vector); }

    public float VectorComparison { get; private set; }
}

public class VectorTextResultItem<TMetadata>
 : VectorTextResultItem<string, TMetadata>, IVectorTextResultItem<TMetadata>
{
    public VectorTextResultItem(IVectorTextItem<string, TMetadata> item, float vectorComparison)
        : base(item, vectorComparison)
    { }
}