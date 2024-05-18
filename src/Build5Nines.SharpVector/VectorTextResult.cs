
namespace Build5Nines.SharpVector;

public interface IVectorTextResult<TMetadata>
{
    IVectorTextResultItem<TMetadata>[] Texts { get; }

    bool HasTexts { get; }
}

public class VectorTextResult<TMetadata> : IVectorTextResult<TMetadata>
{
    public VectorTextResult(IVectorTextResultItem<TMetadata>[] texts){
        Texts = texts;
    }

    public IVectorTextResultItem<TMetadata>[] Texts { get; private set; }

    public bool HasTexts { get => Texts != null && Texts.Length > 0; }
}
