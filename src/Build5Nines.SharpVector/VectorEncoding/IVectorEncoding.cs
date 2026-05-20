namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// A strategy for compressing/encoding float vectors and computing
/// similarity directly against the encoded form.
/// </summary>
/// <remarks>
/// Implementations are responsible for the full round-trip of one encoded
/// vector type: encode a float[] into bytes, restore an encoded vector from
/// bytes, decode back to float[] when needed, and compute similarity between
/// a float query vector and an encoded stored vector for each supported
/// <see cref="VectorComparison"/> metric.
/// </remarks>
public interface IVectorEncoding
{
    /// <summary>
    /// Stable identifier used to tag persisted vectors and look the encoding
    /// up via <see cref="VectorEncodingRegistry"/>.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Encode a float vector into the encoding's compressed form.
    /// </summary>
    IEncodedVector Encode(float[] vector);

    /// <summary>
    /// Reconstruct an encoded vector from previously persisted bytes.
    /// </summary>
    /// <param name="bytes">The raw payload produced by <see cref="IEncodedVector.GetBytes"/>.</param>
    /// <param name="dimensions">The original float vector dimensionality.</param>
    IEncodedVector LoadFromBytes(byte[] bytes, int dimensions);

    /// <summary>
    /// Compute similarity between a float query vector and a stored encoded
    /// vector using the requested metric. Implementations should use the fast
    /// asymmetric path (query stays float, stored stays encoded) whenever
    /// possible; otherwise fall back to <see cref="IEncodedVector.Decode"/>.
    /// </summary>
    float Compare(VectorComparison metric, float[] query, IEncodedVector encoded);
}
