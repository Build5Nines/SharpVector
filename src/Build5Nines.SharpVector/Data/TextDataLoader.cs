namespace Build5Nines.SharpVector.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class TextDataLoader<TId, TMetadata>
    where TId : notnull
    where TMetadata : notnull
{
    public TextDataLoader(IVectorDatabase<TId, TMetadata> vectorDatabase)
    {
        VectorDatabase = vectorDatabase;
    }

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
        var words = text.Split(' ');
        var chunks = new List<string>();

        for (int i = 0; i < words.Length; i += chunkSize)
        {
            chunks.Add(string.Join(" ", words.Skip(i).Take(chunkSize)));
        }

        return chunks;
    }
}
