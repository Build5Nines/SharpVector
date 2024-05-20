namespace Build5Nines.SharpVector.Preprocessing;

public interface ITextPreprocessor
{
    IEnumerable<string> TokenizeAndPreprocess(string text);
    Task<IEnumerable<string>> TokenizeAndPreprocessAsync(string text);
}
