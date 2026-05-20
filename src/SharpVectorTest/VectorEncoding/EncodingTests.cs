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
    public void RaBitQ_StubThrows()
    {
        Assert.ThrowsException<NotImplementedException>(() => RaBitQEncoding.Instance.Encode(new float[4]));
    }

    [TestMethod]
    public void TurboQuant_StubThrows()
    {
        Assert.ThrowsException<NotImplementedException>(() => TurboQuantEncoding.Instance.Encode(new float[4]));
    }
}
