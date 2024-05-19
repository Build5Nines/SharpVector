namespace Build5Nines.SharpVector.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public enum TextChunkingMethod
{
    /// <summary>
    /// Split the text into paragraphs
    /// </summary>
    Paragraph,
    /// <summary>
    /// Split the text into sentences
    /// </summary>
    Sentence,
    /// <summary>
    /// Split the text into fixed length chunks
    /// </summary>
    FixedLength
}

public class TextChunkingOptions<TMetadata>
    where TMetadata : notnull
{
    public TextChunkingOptions()
    {
        Method = TextChunkingMethod.Paragraph;
        ChunkSize = 100;
        RetrieveMetadata = (chunk) => default;
    }

    /// <summary>
    /// The method to use for chunking the text. Default is Paragraph.
    /// </summary>
    public TextChunkingMethod Method { get; set; }

    /// <summary>
    /// The size of each chunk of text. Default is 100.
    /// Used only for FixedLength method
    /// </summary>
    public int ChunkSize { get; set; } 

    /// <summary>
    /// Lambda function to retrieve custom metadata for each chunk
    /// </summary>
    public Func<string, TMetadata> RetrieveMetadata { get; set; }
}

public class TextDataLoader<TId, TMetadata>
    where TId : notnull
    where TMetadata : notnull
{
    public TextDataLoader(IVectorDatabase<TId, TMetadata> vectorDatabase)
    {
        VectorDatabase = vectorDatabase;
    }

    public IVectorDatabase<TId, TMetadata> VectorDatabase { get; set; }

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

    private List<string> ChunkText(string text, TextChunkingOptions<TMetadata> chunkingOptions)
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

    private List<string> SplitIntoParagraphs(string text)
    {
        return text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private List<string> SplitIntoSentences(string text)
    {
        return Regex.Split(text, @"(?<=[\.!\?])\s+").ToList();
    }

    private List<string> SplitIntoChunks(string text, int chunkSize)
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
