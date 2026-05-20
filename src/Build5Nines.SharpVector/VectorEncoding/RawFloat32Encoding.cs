using System;
using System.Buffers.Binary;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Lossless passthrough encoding that stores vectors as their original
/// float32 values. This is the default encoding and produces the same
/// on-disk representation as previous versions of the library when
/// combined with the legacy JSON serializer.
/// </summary>
public sealed class RawFloat32Encoding : IVectorEncoding
{
    public const string EncodingId = "raw-f32";

    public static readonly RawFloat32Encoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector)
    {
        if (vector is null) throw new ArgumentNullException(nameof(vector));
        // Defensive copy so caller mutations don't bleed into the store.
        var copy = new float[vector.Length];
        Buffer.BlockCopy(vector, 0, copy, 0, vector.Length * sizeof(float));
        return new RawEncodedVector(copy);
    }

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length != dimensions * sizeof(float))
            throw new ArgumentException(
                $"Expected {dimensions * sizeof(float)} bytes for {dimensions} float32 values, got {bytes.Length}.",
                nameof(bytes));

        var values = new float[dimensions];
        Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
        return new RawEncodedVector(values);
    }

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded)
    {
        if (encoded is not RawEncodedVector raw)
            raw = new RawEncodedVector(encoded.Decode());

        return metric switch
        {
            VectorComparison.CosineSimilarity => CosineSimilarity(query, raw.Values),
            VectorComparison.EuclideanDistance => EuclideanDistance(query, raw.Values),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };
    }

    internal static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        magA = (float)Math.Sqrt(magA);
        magB = (float)Math.Sqrt(magB);
        if (magA == 0f || magB == 0f) return 0f;
        return dot / (magA * magB);
    }

    internal static float EuclideanDistance(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        float sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            float d = a[i] - b[i];
            sum += d * d;
        }
        return (float)Math.Sqrt(sum);
    }

    private sealed class RawEncodedVector : IEncodedVector
    {
        internal readonly float[] Values;

        public RawEncodedVector(float[] values)
        {
            Values = values;
        }

        public string EncodingId => RawFloat32Encoding.EncodingId;

        public int Dimensions => Values.Length;

        public byte[] GetBytes()
        {
            var bytes = new byte[Values.Length * sizeof(float)];
            Buffer.BlockCopy(Values, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public float[] Decode()
        {
            var copy = new float[Values.Length];
            Buffer.BlockCopy(Values, 0, copy, 0, Values.Length * sizeof(float));
            return copy;
        }
    }
}
