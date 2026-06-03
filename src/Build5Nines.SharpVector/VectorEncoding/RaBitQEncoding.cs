using System;
using System.Buffers.Binary;

namespace Build5Nines.SharpVector.VectorEncoding;

/// <summary>
/// Rotation-free RaBitQ-style 1-bit binary quantization.
/// Each vector is stored as a sign-bit code plus two scalar correction terms
/// (its L2 norm and a per-vector reconstruction factor) so that asymmetric
/// inner-product / cosine similarity against a float query vector can be
/// recovered with reasonable accuracy.
/// </summary>
/// <remarks>
/// The published RaBitQ algorithm pre-rotates database and query vectors with
/// a shared random orthonormal matrix before quantizing, which gives the
/// unbiased estimator strong concentration bounds for arbitrary input
/// distributions. This implementation omits the rotation because the
/// <see cref="IVectorEncoding"/> abstraction is a registry-managed singleton
/// with no per-database state; for already-isotropic embedding outputs (the
/// typical input for this library) the rotation-free estimator is still close
/// to the rotated variant in practice.
///
/// Storage per vector: 8 bytes of scalar correction + ceil(D / 8) bytes of
/// packed sign bits — roughly 1 bit per dimension, a ~32x reduction over
/// raw float32 for high-dimensional embeddings.
/// </remarks>
public sealed class RaBitQEncoding : IVectorEncoding
{
    public const string EncodingId = "rabitq";

    public static readonly RaBitQEncoding Instance = new();

    public string Id => EncodingId;

    public IEncodedVector Encode(float[] vector)
    {
        if (vector is null) throw new ArgumentNullException(nameof(vector));
        int d = vector.Length;

        float normSq = 0f;
        float sumAbs = 0f;
        for (int i = 0; i < d; i++)
        {
            normSq += vector[i] * vector[i];
            sumAbs += MathF.Abs(vector[i]);
        }
        float norm = MathF.Sqrt(normSq);

        // correction = <unit_vector, sign_vector / sqrt(D)>
        //            = (sum(|v_i|) / ||v||) / sqrt(D)
        // For a zero vector both norm and correction are zero; the decoder
        // treats this as a zero estimate.
        float correction = (norm > 0f)
            ? (sumAbs / norm) / MathF.Sqrt(d)
            : 0f;

        var bits = new byte[(d + 7) / 8];
        for (int i = 0; i < d; i++)
        {
            if (vector[i] >= 0f)
            {
                bits[i >> 3] |= (byte)(1 << (i & 7));
            }
        }

        return new RaBitQVector(norm, correction, bits, d);
    }

    public IEncodedVector LoadFromBytes(byte[] bytes, int dimensions)
    {
        if (bytes is null) throw new ArgumentNullException(nameof(bytes));
        int expected = 2 * sizeof(float) + (dimensions + 7) / 8;
        if (bytes.Length != expected)
            throw new ArgumentException(
                $"Expected {expected} bytes for rabitq of {dimensions} dims, got {bytes.Length}.",
                nameof(bytes));

        float norm = BinaryPrimitives.ReadSingleLittleEndian(bytes.AsSpan(0, sizeof(float)));
        float correction = BinaryPrimitives.ReadSingleLittleEndian(bytes.AsSpan(sizeof(float), sizeof(float)));
        var bits = new byte[(dimensions + 7) / 8];
        Buffer.BlockCopy(bytes, 2 * sizeof(float), bits, 0, bits.Length);
        return new RaBitQVector(norm, correction, bits, dimensions);
    }

    public float Compare(VectorComparison metric, float[] query, IEncodedVector encoded)
    {
        if (encoded is not RaBitQVector rab)
            throw new ArgumentException(
                $"RaBitQEncoding cannot compare against encoding '{encoded.EncodingId}'.",
                nameof(encoded));
        if (query.Length != rab.Dimensions)
            throw new ArgumentException("Vectors must be of the same length.");

        // Compute <q, c> where c_i ∈ {+1, -1} from the packed sign bits.
        float qDotC = 0f;
        float qNormSq = 0f;
        var bits = rab.Bits;
        int d = rab.Dimensions;
        for (int i = 0; i < d; i++)
        {
            float sign = ((bits[i >> 3] >> (i & 7)) & 1) == 1 ? 1f : -1f;
            qDotC += query[i] * sign;
            qNormSq += query[i] * query[i];
        }

        // Estimate the cosine between the query and the unit-normalized stored
        // vector: <q_hat, d_hat> ≈ (qDotC / sqrt(D)) / correction, where
        // q_hat = q / ||q||. Multiply through by ||q|| * ||d|| to get the
        // estimated inner product <q, d>.
        float estDot;
        if (rab.Correction > 0f && rab.Norm > 0f)
        {
            float invSqrtD = 1f / MathF.Sqrt(d);
            float estCosineUnit = (qDotC * invSqrtD) / rab.Correction;
            estDot = rab.Norm * estCosineUnit;
        }
        else
        {
            estDot = 0f;
        }

        return metric switch
        {
            VectorComparison.CosineSimilarity => CosineFromEstDot(estDot, qNormSq, rab.Norm),
            VectorComparison.EuclideanDistance => EuclideanFromEstDot(estDot, qNormSq, rab.Norm),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };
    }

    private static float CosineFromEstDot(float estDot, float qNormSq, float dNorm)
    {
        float qNorm = MathF.Sqrt(qNormSq);
        if (qNorm == 0f || dNorm == 0f) return 0f;
        return estDot / (qNorm * dNorm);
    }

    private static float EuclideanFromEstDot(float estDot, float qNormSq, float dNorm)
    {
        // ||q - d||² = ||q||² + ||d||² - 2<q, d>
        float sq = qNormSq + dNorm * dNorm - 2f * estDot;
        if (sq < 0f) sq = 0f;
        return MathF.Sqrt(sq);
    }

    private sealed class RaBitQVector : IEncodedVector
    {
        internal readonly float Norm;
        internal readonly float Correction;
        internal readonly byte[] Bits;

        public RaBitQVector(float norm, float correction, byte[] bits, int dimensions)
        {
            Norm = norm;
            Correction = correction;
            Bits = bits;
            Dimensions = dimensions;
        }

        public string EncodingId => RaBitQEncoding.EncodingId;

        public int Dimensions { get; }

        public byte[] GetBytes()
        {
            var bytes = new byte[2 * sizeof(float) + Bits.Length];
            BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(0, sizeof(float)), Norm);
            BinaryPrimitives.WriteSingleLittleEndian(bytes.AsSpan(sizeof(float), sizeof(float)), Correction);
            Buffer.BlockCopy(Bits, 0, bytes, 2 * sizeof(float), Bits.Length);
            return bytes;
        }

        public float[] Decode()
        {
            // Reconstruct an approximation: each dim recovers as
            //   sign_i * (||d|| / sqrt(D))
            // which is the best single-magnitude reconstruction given only the
            // sign bit and the L2 norm.
            var values = new float[Dimensions];
            float magnitude = (Dimensions > 0) ? Norm / MathF.Sqrt(Dimensions) : 0f;
            for (int i = 0; i < Dimensions; i++)
            {
                float sign = ((Bits[i >> 3] >> (i & 7)) & 1) == 1 ? 1f : -1f;
                values[i] = sign * magnitude;
            }
            return values;
        }
    }
}
