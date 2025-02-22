namespace Build5Nines.SharpVector.Preprocessing;

public interface ITextPreprocessor<TToken>
{
    IEnumerable<TToken> TokenizeAndPreprocess(TToken text);
    Task<IEnumerable<TToken>> TokenizeAndPreprocessAsync(TToken text);
}
