using System;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Placeholder for the TurboQUANT 4-bit block quantization scheme.
/// The faithful algorithm (rotation transform + 4-bit blocks + specialized
/// SIMD distance kernels) is not yet implemented; this stub exists so the
/// encoding id is reserved.
/// </summary>
public sealed class TurboQuantEncoding : IVectorEncoding
{
    public const string EncodingId = "turboquant";

    public static readonly TurboQuantEncoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector) => throw NotImplemented();

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions) => throw NotImplemented();

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded) => throw NotImplemented();

    private static NotImplementedException NotImplemented() =>
        new("TurboQUANT encoding is reserved but not yet implemented. Use RawFloat32Encoding or Int8ScalarQuantizationEncoding.");
}
