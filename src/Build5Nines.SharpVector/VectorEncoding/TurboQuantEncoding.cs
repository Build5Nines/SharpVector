using System;
using System.Buffers.Binary;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// 4-bit symmetric scalar quantization with two codes packed per byte.
/// Each vector stores a single per-vector scale plus a packed nibble stream.
/// Storage shrinks from 4 bytes/dim to ~0.5 bytes/dim (plus a 4-byte scale),
/// roughly an 8x reduction with quality between RaBitQ and int8-sq.
/// </summary>
/// <remarks>
/// The "TurboQUANT" name in the literature refers to several distinct schemes,
/// most including a rotation transform and SIMD-tuned distance kernels. This
/// implementation is the simpler 4-bit symmetric scalar-quantization core that
/// can be swapped for a more elaborate variant later without changing the
/// public surface. Cosine similarity benefits from the scale cancelling in
/// numerator and denominator; Euclidean distance reconstructs values as
/// <c>code * scale</c> before differencing.
/// </remarks>
public sealed class TurboQuantEncoding : IVectorEncoding
{
    public const string EncodingId = "turboquant";

    // Symmetric 4-bit range uses codes -7..+7. The -8 code is not produced by
    // the encoder so the rounding behavior stays symmetric; sign-extending on
    // read still treats 0x8 (= -8) correctly if encountered.
    private const int MaxCode = 7;

    public static readonly TurboQuantEncoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector)
    {
        if (vector is null) throw new ArgumentNullException(nameof(vector));
        int d = vector.Length;

        float absMax = 0f;
        for (int i = 0; i < d; i++)
        {
            float a = MathF.Abs(vector[i]);
            if (a > absMax) absMax = a;
        }

        float scale = absMax / MaxCode;
        var codes = new sbyte[d];
        if (scale > 0f)
        {
            float inv = 1f / scale;
            for (int i = 0; i < d; i++)
            {
                int q = (int)MathF.Round(vector[i] * inv);
                if (q > MaxCode) q = MaxCode;
                else if (q < -MaxCode) q = -MaxCode;
                codes[i] = (sbyte)q;
            }
        }

        return new TurboQuantVector(scale, codes);
    }

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        int expected = sizeof(float) + (dimensions + 1) / 2;
        if (bytes.Length != expected)
            throw new ArgumentException(
                $"Expected {expected} bytes for turboquant of {dimensions} dims, got {bytes.Length}.",
                nameof(bytes));

        float scale = BinaryPrimitives.ReadSingleLittleEndian(bytes.AsSpan(0, sizeof(float)));
        var codes = new sbyte[dimensions];
        int payloadStart = sizeof(float);
        for (int i = 0; i < dimensions; i++)
        {
            byte twoCodes = bytes[payloadStart + (i >> 1)];
            int nibble = ((i & 1) == 0) ? (twoCodes & 0x0F) : ((twoCodes >> 4) & 0x0F);
            // Sign-extend 4-bit value to 8-bit signed.
            if (nibble >= 8) nibble -= 16;
            codes[i] = (sbyte)nibble;
        }
        return new TurboQuantVector(scale, codes);
    }

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded)
    {
        if (encoded is not TurboQuantVector tq)
            throw new ArgumentException(
                $"TurboQuantEncoding cannot compare against encoding '{encoded.EncodingId}'.",
                nameof(encoded));
        if (query.Length != tq.Codes.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        return metric switch
        {
            VectorComparison.CosineSimilarity => CosineSimilarity(query, tq),
            VectorComparison.EuclideanDistance => EuclideanDistance(query, tq),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };
    }

    private static float CosineSimilarity(float[] query, TurboQuantVector stored)
    {
        // Identical reasoning to Int8 SQ: the per-vector scale appears in both
        // numerator and the magnitude of the decoded vector, so it cancels.
        float dot = 0f;
        long codeSqSum = 0;
        float qSqSum = 0f;
        var codes = stored.Codes;
        for (int i = 0; i < codes.Length; i++)
        {
            float qi = query[i];
            int ci = codes[i];
            dot += qi * ci;
            codeSqSum += ci * ci;
            qSqSum += qi * qi;
        }
        float magQuery = MathF.Sqrt(qSqSum);
        float magCodes = MathF.Sqrt(codeSqSum);
        if (magQuery == 0f || magCodes == 0f) return 0f;
        return dot / (magQuery * magCodes);
    }

    private static float EuclideanDistance(float[] query, TurboQuantVector stored)
    {
        float scale = stored.Scale;
        var codes = stored.Codes;
        float sum = 0f;
        for (int i = 0; i < codes.Length; i++)
        {
            float d = query[i] - codes[i] * scale;
            sum += d * d;
        }
        return MathF.Sqrt(sum);
    }

    private sealed class TurboQuantVector : IEncodedVector
    {
        internal readonly float Scale;
        internal readonly sbyte[] Codes;

        public TurboQuantVector(float scale, sbyte[] codes)
        {
            Scale = scale;
            Codes = codes;
        }

        public string EncodingId => TurboQuantEncoding.EncodingId;

        public int Dimensions => Codes.Length;

        public byte[] GetBytes()
        {
            int d = Codes.Length;
            int payloadLen = (d + 1) / 2;
            var bytes = new byte[sizeof(float) + payloadLen];
            BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0, sizeof(float)), Scale);

            int payloadStart = sizeof(float);
            for (int i = 0; i < d; i++)
            {
                int nibble = Codes[i] & 0x0F; // mask to 4 bits (two's complement preserved)
                if ((i & 1) == 0)
                {
                    bytes[payloadStart + (i >> 1)] = (byte)nibble;
                }
                else
                {
                    bytes[payloadStart + (i >> 1)] |= (byte)(nibble << 4);
                }
            }
            return bytes;
        }

        public float[] Decode()
        {
            var values = new float[Codes.Length];
            for (int i = 0; i < Codes.Length; i++)
            {
                values[i] = Codes[i] * Scale;
            }
            return values;
        }
    }
}
