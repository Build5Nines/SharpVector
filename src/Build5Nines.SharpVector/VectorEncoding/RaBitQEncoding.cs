using System;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Placeholder for the RaBitQ binary quantization scheme. The faithful
/// algorithm (random rotation + 1-bit sign codes + unbiased dot-product
/// estimator) is not yet implemented; this stub exists so the encoding id
/// is reserved and persisted databases can fail with a clear message until
/// a real implementation lands.
/// </summary>
public sealed class RaBitQEncoding : IVectorEncoding
{
    public const string EncodingId = "rabitq";

    public static readonly RaBitQEncoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector) => throw NotImplemented();

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions) => throw NotImplemented();

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded) => throw NotImplemented();

    private static NotImplementedException NotImplemented() =>
        new("RaBitQ encoding is reserved but not yet implemented. Use RawFloat32Encoding or Int8ScalarQuantizationEncoding.");
}
