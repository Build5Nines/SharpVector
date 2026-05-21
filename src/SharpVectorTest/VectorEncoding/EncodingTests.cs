using Build5Nines.SharpVector;
using Build5Nines.SharpVector.VectorEncoding;

namespace SharpVectorTest.VectorEncoding;

[TestClass]
public class EncodingTests
{
    private static float[] SampleVector(int dims, int seed)
    {
        var rng = new Random(seed);
        var v = new float[dims];
        for (int i = 0; i < dims; i++) v[i] = (float)(rng.NextDouble() * 2.0 - 1.0);
        return v;
    }

    [TestMethod]
    public void RawFloat32_RoundTrip_IsLossless()
    {
        var original = SampleVector(128, 1);
        var encoded = RawFloat32Encoding.Instance.Encode(original);
        Assert.AreEqual(RawFloat32Encoding.EncodingId, encoded.EncodingId);
        Assert.AreEqual(128, encoded.Dimensions);

        var decoded = encoded.Decode();
        CollectionAssert.AreEqual(original, decoded);
    }

    [TestMethod]
    public void RawFloat32_BytesRoundTrip_IsLossless()
    {
        var original = SampleVector(64, 2);
        var encoded = RawFloat32Encoding.Instance.Encode(original);
        var bytes = encoded.GetBytes();

        var rehydrated = RawFloat32Encoding.Instance.LoadFromBytes(bytes, 64);
        CollectionAssert.AreEqual(original, rehydrated.Decode());
    }

    [TestMethod]
    public void Int8Sq_RoundTrip_PreservesCosineSimilarityClosely()
    {
        var a = SampleVector(384, 3);
        var b = SampleVector(384, 4);

        var rawEncA = RawFloat32Encoding.Instance.Encode(a);
        var rawCos = RawFloat32Encoding.Instance.Compare(VectorComparison.CosineSimilarity, b, rawEncA);

        var encA = Int8ScalarQuantizationEncoding.Instance.Encode(a);
        var cosViaInt8 = Int8ScalarQuantizationEncoding.Instance.Compare(
            VectorComparison.CosineSimilarity, b, encA);

        // int8-sq introduces small rounding error; for random vectors this is well under 1%.
        Assert.IsTrue(Math.Abs(rawCos - cosViaInt8) < 0.01f,
            $"Expected cosine within 0.01 of raw value, got |{rawCos} - {cosViaInt8}| = {Math.Abs(rawCos - cosViaInt8)}");
    }

    [TestMethod]
    public void Int8Sq_BytesRoundTrip_RestoresEncodedForm()
    {
        var original = SampleVector(256, 5);
        var encoded = Int8ScalarQuantizationEncoding.Instance.Encode(original);
        var bytes = encoded.GetBytes();

        var rehydrated = Int8ScalarQuantizationEncoding.Instance.LoadFromBytes(bytes, 256);

        // Compare cosine sim against an arbitrary query: should be identical
        // since the encoded form is fully recovered.
        var query = SampleVector(256, 6);
        float a = Int8ScalarQuantizationEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encoded);
        float b = Int8ScalarQuantizationEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, rehydrated);
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void Registry_ResolvesBuiltinEncodings()
    {
        Assert.AreSame(RawFloat32Encoding.Instance, VectorEncodingRegistry.Get(RawFloat32Encoding.EncodingId));
        Assert.AreSame(Int8ScalarQuantizationEncoding.Instance, VectorEncodingRegistry.Get(Int8ScalarQuantizationEncoding.EncodingId));
    }

    [TestMethod]
    public void Registry_UnknownIdThrows()
    {
        Assert.ThrowsException<KeyNotFoundException>(() => VectorEncodingRegistry.Get("no-such-encoding"));
    }

    [TestMethod]
    public void RaBitQ_BytesRoundTrip_RestoresEncodedForm()
    {
        var original = SampleVector(256, 7);
        var encoded = RaBitQEncoding.Instance.Encode(original);
        var bytes = encoded.GetBytes();

        var rehydrated = RaBitQEncoding.Instance.LoadFromBytes(bytes, 256);

        var query = SampleVector(256, 8);
        float a = RaBitQEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encoded);
        float b = RaBitQEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, rehydrated);
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void RaBitQ_RankingMatchesRawForRandomVectors()
    {
        // RaBitQ is a coarse approximation, so don't assert numeric closeness;
        // instead assert it can still rank a clearly-similar pair above a
        // clearly-dissimilar pair.
        var query = SampleVector(512, 10);
        var similar = (float[])query.Clone();
        for (int i = 0; i < similar.Length; i++) similar[i] += 0.05f * (i % 3 - 1);
        var different = SampleVector(512, 11);

        var encSimilar = RaBitQEncoding.Instance.Encode(similar);
        var encDifferent = RaBitQEncoding.Instance.Encode(different);

        float simSimilar = RaBitQEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encSimilar);
        float simDifferent = RaBitQEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encDifferent);

        Assert.IsTrue(simSimilar > simDifferent,
            $"RaBitQ similarity ranking incorrect: similar={simSimilar}, different={simDifferent}");
    }

    [TestMethod]
    public void RaBitQ_ProducesExpectedStorageSize()
    {
        // 8 bytes of scalar correction + ceil(D/8) bytes of sign bits.
        var encoded = RaBitQEncoding.Instance.Encode(SampleVector(384, 12));
        Assert.AreEqual(8 + 48, encoded.GetBytes().Length);
    }

    [TestMethod]
    public void TurboQuant_RoundTrip_PreservesCosineSimilarityClosely()
    {
        var a = SampleVector(384, 13);
        var b = SampleVector(384, 14);

        var rawEncA = RawFloat32Encoding.Instance.Encode(a);
        var rawCos = RawFloat32Encoding.Instance.Compare(VectorComparison.CosineSimilarity, b, rawEncA);

        var encA = TurboQuantEncoding.Instance.Encode(a);
        var cosViaTurbo = TurboQuantEncoding.Instance.Compare(VectorComparison.CosineSimilarity, b, encA);

        // 4-bit SQ is coarser than int8; allow up to 3% deviation for random vectors.
        Assert.IsTrue(Math.Abs(rawCos - cosViaTurbo) < 0.03f,
            $"Expected cosine within 0.03 of raw value, got |{rawCos} - {cosViaTurbo}| = {Math.Abs(rawCos - cosViaTurbo)}");
    }

    [TestMethod]
    public void TurboQuant_BytesRoundTrip_RestoresEncodedForm()
    {
        var original = SampleVector(256, 15);
        var encoded = TurboQuantEncoding.Instance.Encode(original);
        var bytes = encoded.GetBytes();

        var rehydrated = TurboQuantEncoding.Instance.LoadFromBytes(bytes, 256);

        var query = SampleVector(256, 16);
        float a = TurboQuantEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encoded);
        float b = TurboQuantEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, rehydrated);
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void TurboQuant_HandlesOddDimensions()
    {
        // Odd dimension count exercises the nibble-packing tail case.
        var original = SampleVector(127, 17);
        var encoded = TurboQuantEncoding.Instance.Encode(original);
        var bytes = encoded.GetBytes();
        // 4 bytes scale + ceil(127/2) = 64 bytes payload
        Assert.AreEqual(4 + 64, bytes.Length);

        var rehydrated = TurboQuantEncoding.Instance.LoadFromBytes(bytes, 127);
        var query = SampleVector(127, 18);
        float a = TurboQuantEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, encoded);
        float b = TurboQuantEncoding.Instance.Compare(VectorComparison.CosineSimilarity, query, rehydrated);
        Assert.AreEqual(a, b);
    }
}
