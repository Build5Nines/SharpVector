using System;
using System.Buffers.Binary;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Symmetric per-vector int8 scalar quantization. Each vector chooses its own
/// scale equal to its absolute-max value divided by 127, then quantizes every
/// component into a signed byte. Storage shrinks from 4 bytes/dim to roughly
/// 1 byte/dim (plus a single 4-byte scale per vector).
/// </summary>
/// <remarks>
/// For cosine similarity the per-vector scale cancels out, so quality loss is
/// limited to the rounding error of the int8 codes. Euclidean distance
/// reconstructs values as <c>code * scale</c> before differencing.
/// </remarks>
public sealed class Int8ScalarQuantizationEncoding : IVectorEncoding
{
    public const string EncodingId = "int8-sq";

    public static readonly Int8ScalarQuantizationEncoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector)
    {
        if (vector is null) throw new ArgumentNullException(nameof(vector));

        float absMax = 0f;
        for (int i = 0; i < vector.Length; i++)
        {
            float a = Math.Abs(vector[i]);
            if (a > absMax) absMax = a;
        }

        // A zero vector encodes as all-zero codes with scale 0; decode round-trips to zero.
        float scale = absMax / 127f;
        var codes = new sbyte[vector.Length];
        if (scale > 0f)
        {
            float inv = 1f / scale;
            for (int i = 0; i < vector.Length; i++)
            {
                int q = (int)MathF.Round(vector[i] * inv);
                if (q > 127) q = 127;
                else if (q < -127) q = -127;
                codes[i] = (sbyte)q;
            }
        }

        return new Int8EncodedVector(scale, codes);
    }

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length != sizeof(float) + dimensions)
            throw new ArgumentException(
                $"Expected {sizeof(float) + dimensions} bytes for int8-sq of {dimensions} dims, got {bytes.Length}.",
                nameof(bytes));

        float scale = BinaryPrimitives.ReadSingleLittleEndian(bytes.AsSpan(0, sizeof(float)));
        var codes = new sbyte[dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            codes[i] = (sbyte)bytes[sizeof(float) + i];
        }
        return new Int8EncodedVector(scale, codes);
    }

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded)
    {
        if (encoded is not Int8EncodedVector q)
            throw new ArgumentException(
                $"Int8ScalarQuantizationEncoding cannot compare against encoding '{encoded.EncodingId}'.",
                nameof(encoded));

        if (query.Length != q.Codes.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        return metric switch
        {
            VectorComparison.CosineSimilarity => CosineSimilarity(query, q),
            VectorComparison.EuclideanDistance => EuclideanDistance(query, q),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };
    }

    private static float CosineSimilarity(float[] query, Int8EncodedVector stored)
    {
        // cos = sum(q_i * c_i) / (|query| * sqrt(sum(c_i^2)))
        // The scale factor cancels because it appears identically in numerator and
        // in the magnitude of the decoded stored vector.
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
        float magQuery = (float)Math.Sqrt(qSqSum);
        float magCodes = (float)Math.Sqrt(codeSqSum);
        if (magQuery == 0f || magCodes == 0f) return 0f;
        return dot / (magQuery * magCodes);
    }

    private static float EuclideanDistance(float[] query, Int8EncodedVector stored)
    {
        float scale = stored.Scale;
        var codes = stored.Codes;
        float sum = 0f;
        for (int i = 0; i < codes.Length; i++)
        {
            float d = query[i] - codes[i] * scale;
            sum += d * d;
        }
        return (float)Math.Sqrt(sum);
    }

    private sealed class Int8EncodedVector : IEncodedVector
    {
        internal readonly float Scale;
        internal readonly sbyte[] Codes;

        public Int8EncodedVector(float scale, sbyte[] codes)
        {
            Scale = scale;
            Codes = codes;
        }

        public string EncodingId => Int8ScalarQuantizationEncoding.EncodingId;

        public int Dimensions => Codes.Length;

        public byte[] GetBytes()
        {
            var bytes = new byte[sizeof(float) + Codes.Length];
            BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0, sizeof(float)), Scale);
            for (int i = 0; i < Codes.Length; i++)
            {
                bytes[sizeof(float) + i] = (byte)Codes[i];
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
