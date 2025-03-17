namespace Build5Nines.SharpVector.Data;

public class TextChunkingOptions<TMetadata>
{
    public TextChunkingOptions()
    {
        Method = TextChunkingMethod.Paragraph;
        ChunkSize = 100;
#pragma warning disable CS8603 // Possible null reference return.
        RetrieveMetadata = (chunk) => default;
#pragma warning restore CS8603 // Possible null reference return.
        OverlapSize = 50;
    }

    /// <summary>
    /// The method to use for chunking the text. Default is Paragraph.
    /// </summary>
    public TextChunkingMethod Method { get; set; }

    /// <summary>
    /// The length in tokens (aka "words") of each chunk of text. Default is 100.
    /// Only used by TextChunkingMethod.FixedLength and TextChunkingMethod.OverlappingWindow.
    /// </summary>
    public int ChunkSize { get; set; }

    /// <summary>
    /// Lambda function to retrieve custom metadata for each chunk
    /// </summary>
    public Func<string, TMetadata> RetrieveMetadata { get; set; }

    /// <summary>
    /// The number of words to overlap text chunks when using using TextChunkingMethod.OverlappingWindow. Default is 50.
    /// </summary>
    public int OverlapSize { get; set; }
}
