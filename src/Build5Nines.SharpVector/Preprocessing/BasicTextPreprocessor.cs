namespace Build5Nines.SharpVector.Preprocessing;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public class BasicTextPreprocessor : ITextPreprocessor<string>
{
    private const string space = " ";
    private const char charSpace = ' ';

    private const string regexChineseCharactersPattern = @"\p{IsCJKUnifiedIdeographs}";
    private const string regexRemovePunctuation = @"[\p{P}$^`~=+|<>]"; // @"[\p{P}]";
    // private const string regexTokenize = @"[\p{IsCJKUnifiedIdeographs}]|\p{So}\p{Sk}|[a-z0-9]+";
    private const string regexWhitespacePattern = @"\s+";
    private const string regexEmojiPattern = @"[\p{So}\uD83C-\uDBFF\uDC00-\uDFFF]";    

    public IEnumerable<string> TokenizeAndPreprocess(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();

        // Tokens should always be lower case
        text = text.ToLower();

        // Remove punctuation (excluding Chinese characters)
        text = Regex.Replace(text, regexRemovePunctuation, string.Empty);

        // Space pad special characters (Emoji and Chinese characters)
        text = SpacePadSpecialCharacters(text);

        // Remove extra whitespace characters
        text = Regex.Replace(text, regexWhitespacePattern, space).Trim();   

        // Split to Token array
        return text.Split(charSpace);


        // // Check if text contains Chinese characters using the CJK Unified Ideographs block
        // if (Regex.IsMatch(text, regexChineseCharactersPattern))
        // {
        //     if (Regex.IsMatch(text, regexEmojiPattern))
        //     {
        //         // Has Emoji
        //         text = SpacePadSpecialCharacters(text, new string[] { regexEmojiPattern, regexChineseCharactersPattern });
        //         // remove extra whitespace characters
        //         text = Regex.Replace(text, regexWhitespacePattern, space).Trim();
        //     } else {
        //         // No Emoji
        //         // Tokenize either by matching individual Chinese characters or contiguous word tokens (for Latin letters/digits)
        //         var tokens = Regex.Matches(text, regexTokenize)
        //                         .Cast<Match>()
        //                         .Select(m => m.Value);
        //         return tokens;
        //     }
        // }
        // else
        // {
        //     // if text contains emojis
        //     if (Regex.IsMatch(text, regexEmojiPattern))
        //     {
        //         text = SpacePadSpecialCharacters(text, new string[] { regexEmojiPattern });
        //     }
            
        //     // remove extra whitespace characters
        //     text = Regex.Replace(text, regexWhitespacePattern, space).Trim();   
        // }

        // return text.Split(charSpace);
    }

    public async Task<IEnumerable<string>> TokenizeAndPreprocessAsync(string text)
    {
        return await Task.Run(() => TokenizeAndPreprocess(text));
    }


    private static string SpacePadSpecialCharacters(string text)
    {
        var spacePadPatterns = new List<string>();

        // Contains Chinese characters?
        if (Regex.IsMatch(text, regexChineseCharactersPattern))
        {
            // Space pad Chinese characters
            spacePadPatterns.Add(regexChineseCharactersPattern);
        }

        // Contains Emoji?
        if (Regex.IsMatch(text, regexEmojiPattern))
        {
            // Space pad Emoji characters
            spacePadPatterns.Add(regexEmojiPattern);
        }

        if (spacePadPatterns.Count > 0)
        {
            // Space pad special characters based on the patterns selected
            text = SpacePadSpecialCharacters(text, spacePadPatterns.ToArray());
        }

        return text;
    }

    private static string SpacePadSpecialCharacters(string text, string[] regexPatterns)
    {
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
