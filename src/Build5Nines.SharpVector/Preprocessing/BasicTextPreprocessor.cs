namespace Build5Nines.SharpVector.Preprocessing;

using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

public class BasicTextPreprocessor : ITextPreprocessor<string>
{
    private const string space = " ";
    private const char charSpace = ' ';

    private const string regexChineseCharactersPattern = @"\p{IsCJKUnifiedIdeographs}";
    private const string regexRemovePunctuation = @"[\p{P}$^`~=+|<>]"; // @"[\p{P}]";
    private const string regexTokenize = @"[\p{IsCJKUnifiedIdeographs}]|\p{So}\p{Sk}|[a-z0-9]+";
    private const string regexWhitespacePattern = @"\s+";
    private const string regexEmojiPattern = @"[\p{So}\uD83C-\uDBFF\uDC00-\uDFFF]";    

    public IEnumerable<string> TokenizeAndPreprocess(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();

        text = text.ToLower();

        // Remove punctuation (excluding Chinese characters)
        text = Regex.Replace(text, regexRemovePunctuation, string.Empty);
        //text = Regex.Replace(text, @"[$^`~=+|<>]", space);

        Console.WriteLine($"Text after removing punctuation: {text}");

        // Check if text contains Chinese characters using the CJK Unified Ideographs block
        if (Regex.IsMatch(text, regexChineseCharactersPattern))
        {
            if (Regex.IsMatch(text, regexEmojiPattern))
            {
                // Has Emoji
                text = SpacePadSpecialCharacters(text, new string[] { regexEmojiPattern, regexChineseCharactersPattern });
                // remove extra whitespace characters
                text = Regex.Replace(text, regexWhitespacePattern, space).Trim();
            } else {
                // No Emoji
                // Tokenize either by matching individual Chinese characters or contiguous word tokens (for Latin letters/digits)
                var tokens = Regex.Matches(text, regexTokenize)
                                .Cast<Match>()
                                .Select(m => m.Value);
                return tokens;
            }
        }
        else
        {
            // if text contains emojis
            if (Regex.IsMatch(text, regexEmojiPattern))
            {
                text = SpacePadSpecialCharacters(text, new string[] { regexEmojiPattern });
            }
            
            // remove extra whitespace characters
            text = Regex.Replace(text, regexWhitespacePattern, space).Trim();   
        }

        return text.Split(charSpace);
    }

    public async Task<IEnumerable<string>> TokenizeAndPreprocessAsync(string text)
    {
        return await Task.Run(() => TokenizeAndPreprocess(text));
    }


    private static string SpacePadSpecialCharacters(string text, string[] regexPatterns){
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        StringBuilder sb = new StringBuilder();
        int i;
        while(enumerator.MoveNext())
        {
            var element = enumerator.GetTextElement();

            for (i = 0; i < regexPatterns.Length; i++)
            {
                if (Regex.IsMatch(element, regexPatterns[i]))
                {
                    element = space + element + space;
                    break;
                }
            }

            sb.Append(element);
        }
        return sb.ToString();
    }
}
