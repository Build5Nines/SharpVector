namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// A vector that has been encoded by an <see cref="IVectorEncoding"/>.
/// The encoded form is what the database actually stores; the original
/// floating-point values are recoverable via <see cref="Decode"/>.
/// </summary>
public interface IEncodedVector
{
    /// <summary>
    /// Identifier of the encoding that produced this vector. Used to look up
    /// the matching <see cref="IVectorEncoding"/> when deserializing.
    /// </summary>
    string EncodingId { get; }

    /// <summary>
    /// Logical dimensionality of the original float vector.
    /// </summary>
    int Dimensions { get; }

    /// <summary>
    /// Raw bytes of the encoded payload, suitable for persistence.
    /// </summary>
    byte[] GetBytes();

    /// <summary>
    /// Reconstructs an approximation of the original float vector.
    /// For lossless encodings this is exact; for compressed encodings it
    /// is a lossy approximation.
    /// </summary>
    float[] Decode();
}
