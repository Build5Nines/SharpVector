namespace Build5Nines.SharpVector.Data;

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