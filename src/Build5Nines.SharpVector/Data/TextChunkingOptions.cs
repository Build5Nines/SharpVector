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
