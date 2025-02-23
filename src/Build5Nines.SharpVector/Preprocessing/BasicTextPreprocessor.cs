namespace Build5Nines.SharpVector.Preprocessing;

using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public class BasicTextPreprocessor : ITextPreprocessor<string>
{
    public IEnumerable<string> TokenizeAndPreprocess(string text)
    {
        text = text.ToLower();

        // Check if text contains Chinese characters using the CJK Unified Ideographs block
        if (Regex.IsMatch(text, @"\p{IsCJKUnifiedIdeographs}"))
        {
            // Remove punctuation (excluding Chinese characters)
            text = Regex.Replace(text, @"[^\p{IsCJKUnifiedIdeographs}\w\s]", "");
            // Tokenize either by matching individual Chinese characters or contiguous word tokens (for Latin letters/digits)
            var tokens = Regex.Matches(text, @"[\p{IsCJKUnifiedIdeographs}]|[a-z0-9]+")
                              .Cast<Match>()
                              .Select(m => m.Value);
            return tokens.ToList();
        }
        else
        {
            text = Regex.Replace(text, @"[^\w\s]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text.Split(' ').ToList();
        }
    }

    public async Task<IEnumerable<string>> TokenizeAndPreprocessAsync(string text)
    {
        return await Task.Run(() => TokenizeAndPreprocess(text));
    }
}
