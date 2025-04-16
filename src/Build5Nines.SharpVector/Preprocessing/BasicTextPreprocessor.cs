namespace Build5Nines.SharpVector.Preprocessing;

using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public class BasicTextPreprocessor : ITextPreprocessor<string>
{
    private const string space = " ";
    private const char charSpace = ' ';

    private const string regexMatchChineseCharacters = @"\p{IsCJKUnifiedIdeographs}";
    private const string regexRemovePunctuation = @"[^\p{IsCJKUnifiedIdeographs}\w\s]";
    private const string regexTokenize = @"[\p{IsCJKUnifiedIdeographs}]|[a-z0-9]+";
    private const string regexWhitespace = @"\s+";
    private const string regexNotAWord = @"[^\w\s]";

    public IEnumerable<string> TokenizeAndPreprocess(string text)
    {
        text = text.ToLower();

        // Check if text contains Chinese characters using the CJK Unified Ideographs block
        if (Regex.IsMatch(text, regexMatchChineseCharacters))
        {
            // Remove punctuation (excluding Chinese characters)
            text = Regex.Replace(text, regexRemovePunctuation, string.Empty);
            // Tokenize either by matching individual Chinese characters or contiguous word tokens (for Latin letters/digits)
            var tokens = Regex.Matches(text, regexTokenize)
                              .Cast<Match>()
                              .Select(m => m.Value);
            return tokens;
        }
        else
        {
            text = Regex.Replace(text, regexNotAWord, string.Empty);
            text = Regex.Replace(text, regexWhitespace, space).Trim();
            return text.Split(charSpace);
        }
    }

    public async Task<IEnumerable<string>> TokenizeAndPreprocessAsync(string text)
    {
        return await Task.Run(() => TokenizeAndPreprocess(text));
    }
}
