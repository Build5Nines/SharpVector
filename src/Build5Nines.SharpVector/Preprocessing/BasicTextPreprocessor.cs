namespace Build5Nines.SharpVector.Preprocessing;

using System.Text.RegularExpressions;

public class BasicTextPreprocessor : ITextPreprocessor
{
    public IEnumerable<string> TokenizeAndPreprocess(string text)
    {
        text = text.ToLower();
        text = Regex.Replace(text, @"[^\w\s]", "");
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text.Split(' ').ToList();
    }
}
