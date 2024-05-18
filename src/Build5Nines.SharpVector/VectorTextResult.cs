
public interface IVectorTextResult<TMetadata>
{
    VectorTextResultItem<TMetadata>[] Texts { get; }

    bool HasTexts { get; }
}

public class VectorTextResult<TMetadata> : IVectorTextResult<TMetadata>
{
    public VectorTextResult(VectorTextResultItem<TMetadata>[] texts){
        Texts = texts;
    }

    public VectorTextResultItem<TMetadata>[] Texts { get; private set; }

    public bool HasTexts { get => Texts != null && Texts.Length > 0; }
}


public interface IVectorTextResultItem<TMetadata>
{
    string Text{ get; }
    TMetadata Metadata { get; }
}

public class VectorTextResultItem<TMetadata> : IVectorTextResultItem<TMetadata>
{
    private VectorTextItem<TMetadata> _item;
    public VectorTextResultItem(VectorTextItem<TMetadata> item){
        _item = item;
    }
    public string Text { get => _item.Text; }
    public TMetadata Metadata { get => _item.Metadata; }
}