using System.Collections.Immutable;
using Build5Nines.SharpVector.Id;

namespace Build5Nines.SharpVector;

public interface IVectorTextResultItem<TDocument, TMetadata>
{
    TDocument Text{ get; }
    TMetadata? Metadata { get; }

    float VectorComparison { get; }
}

public interface IVectorTextResultItem<TId, TDocument, TMetadata>
    : IVectorTextResultItem<TDocument, TMetadata>
{
    TId Id { get; }
}

public interface IVectorTextResultItem<TMetadata>
 : IVectorTextResultItem<string, TMetadata>, IVectorTextResultItem<int, string, TMetadata>
{ }

public class VectorTextResultItem<TId, TDocument, TMetadata>
    : IVectorTextResultItem<TDocument, TMetadata>, IVectorTextResultItem<TId, TDocument, TMetadata>
{
    private IVectorTextItem<TDocument, TMetadata> _item;
    private TId _id;

    public VectorTextResultItem(TId id, IVectorTextItem<TDocument, TMetadata> item, float vectorComparison)
    {
        _id = id;
        _item = item;
        VectorComparison = vectorComparison;
    }
    
    public TDocument Text { get => _item.Text; }
    public TMetadata? Metadata { get => _item.Metadata; }
    public TId Id { get => _id; }

    public ImmutableArray<float> Vectors { get => ImmutableArray.Create(_item.Vector); }

    public float VectorComparison { get; private set; }
}

public class VectorTextResultItem<TMetadata>
 : VectorTextResultItem<int, string, TMetadata>, IVectorTextResultItem<TMetadata>
{
    public VectorTextResultItem(int id, IVectorTextItem<string, TMetadata> item, float vectorComparison)
        : base(id, item, vectorComparison)
    { }
}

