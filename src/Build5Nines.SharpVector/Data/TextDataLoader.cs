namespace Build5Nines.SharpVector.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Build5Nines.SharpVector.Preprocessing;

public class TextDataLoader<TId, TMetadata>
    where TId : notnull
    where TMetadata : notnull
{
    public TextDataLoader(IVectorDatabase<TId, TMetadata> vectorDatabase)
    {
        VectorDatabase = vectorDatabase;
    }

    const string _space = " ";

    public IVectorDatabase<TId, TMetadata> VectorDatabase { get; private set; }

    public IEnumerable<TId> AddDocument(string document, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        if (chunkingOptions.RetrieveMetadata == null)
            throw new ValidationException("TextChunkingOptions.RetrieveMetadata must be set");

        var chunks = ChunkText(document, chunkingOptions);
        var ids = new List<TId>();

        foreach (var chunk in chunks)
        {
            var id = VectorDatabase.AddText(chunk, chunkingOptions.RetrieveMetadata.Invoke(chunk));
            ids.Add(id);
        }

        return ids;
    }

    protected List<string> ChunkText(string text, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        switch (chunkingOptions.Method)
        {
            case TextChunkingMethod.Paragraph:
                return SplitIntoParagraphs(text);
            case TextChunkingMethod.Sentence:
                return SplitIntoSentences(text);
            case TextChunkingMethod.FixedLength:
                return SplitIntoChunks(text, chunkingOptions.ChunkSize);
            case TextChunkingMethod.OverlappingWindow:
                return SplitIntoOverlappingWindows(text, chunkingOptions.ChunkSize, chunkingOptions.OverlapSize);
            default:
                throw new ArgumentException("Invalid chunking method");
        }
    }

    protected static List<string> SplitIntoParagraphs(string text)
    {
        return text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    protected static List<string> SplitIntoSentences(string text)
    {
        return Regex.Split(text, @"(?<=[\.!\?])\s+").ToList();
    }

    protected static List<string> SplitIntoChunks(string text, int chunkSize)
    {
        var words = SplitIntoTokens(text);
        var chunks = new List<string>();

        for (int i = 0; i < words.Length; i += chunkSize)
        {
            chunks.Add(JoinTokens(words.Skip(i).Take(chunkSize)));
        }

        return chunks;
    }

    protected static List<string> SplitIntoOverlappingWindows(string text, int chunkSize, int overlap)
    {
        var tokens = SplitIntoTokens(text);
        var chunks = new List<string>();

        if (overlap >= chunkSize)
            throw new ArgumentException("Overlap must be smaller than chunk size");

        // Calculate the step size
        int step = chunkSize - overlap;
        int tokenLength = tokens.Length;
        for (int i = 0; i < tokenLength; i += step)
        {
            var chunk = JoinTokens(tokens.Skip(i).Take(chunkSize));
            if (!string.IsNullOrWhiteSpace(chunk))
                chunks.Add(chunk);

            if (i + chunkSize >= tokenLength)
                break;
        }
        return chunks;
    }

    private static string JoinTokens(IEnumerable<string> tokens)
    {
        if (tokens == null) return string.Empty;

        var fullText = new System.Text.StringBuilder();
        foreach (var token in tokens)
        {
            if (IsChinese(token))
                fullText.Append(token);
            else
                fullText.Append(_space + token);
        }
        return fullText.ToString().Trim();
    }

    private static bool IsChinese(string token)
    {
        // Checks if the token consists entirely of Chinese (CJK Unified Ideograph) characters.
        return System.Text.RegularExpressions.Regex.IsMatch(token, @"^\p{IsCJKUnifiedIdeographs}+$");
    }

    protected static string[] SplitIntoTokens(string text)
    {
        var processor = new BasicTextPreprocessor();
        return processor.TokenizeAndPreprocess(text).ToArray();
    }

    public async Task<IEnumerable<TId>> AddDocumentAsync(string document, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        if (chunkingOptions.RetrieveMetadata == null)
            throw new ValidationException("TextChunkingOptions.RetrieveMetadata must be set");

        var chunks = await ChunkTextAsync(document, chunkingOptions);
        var ids = new List<TId>();
        object _lock = new object();
        await Parallel.ForEachAsync(chunks, async (chunk, cancellationToken) =>
        {
            var id = await VectorDatabase.AddTextAsync(chunk, chunkingOptions.RetrieveMetadata.Invoke(chunk));
            lock (_lock) {
                ids.Add(id);
            }
        });

        return ids;
    }

    private async Task<List<string>> ChunkTextAsync(string text, TextChunkingOptions<TMetadata> chunkingOptions)
    {
        return await Task.Run(() => ChunkText(text, chunkingOptions));
    }
}
