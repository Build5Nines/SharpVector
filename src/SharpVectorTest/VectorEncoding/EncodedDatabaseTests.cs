using Build5Nines.SharpVector;
using Build5Nines.SharpVector.VectorEncoding;

namespace SharpVectorTest.VectorEncoding;

[TestClass]
public class EncodedDatabaseTests
{
    private const string SampleText =
        "The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.";

    [TestMethod]
    public void DefaultDatabaseUsesRawEncoding()
    {
        var vdb = new BasicMemoryVectorDatabase();
        Assert.AreEqual(RawFloat32Encoding.EncodingId, vdb.VectorEncoding.Id);
    }

    [TestMethod]
    public void ConstructorAcceptsEncoding()
    {
        var vdb = new BasicMemoryVectorDatabase(Int8ScalarQuantizationEncoding.Instance);
        Assert.AreEqual(Int8ScalarQuantizationEncoding.EncodingId, vdb.VectorEncoding.Id);
    }

    [TestMethod]
    public void Int8Database_StoresEncodedVectorAndStillFindsResult()
    {
        var vdb = new BasicMemoryVectorDatabase(Int8ScalarQuantizationEncoding.Instance);
        vdb.AddText(SampleText, "meta");

        var stored = vdb.GetText(1);
        Assert.AreEqual(Int8ScalarQuantizationEncoding.EncodingId, stored.EncodedVector.EncodingId);

        var results = vdb.Search("Lion King");
        Assert.IsTrue(results.Texts.Any(t => t.Text.Contains("Lion King")));
    }

    [TestMethod]
    public async Task SaveAndLoad_PreservesRawEncoding()
    {
        var vdb = new BasicMemoryVectorDatabase();
        vdb.AddText(SampleText, "meta");

        using var ms = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(ms);
        ms.Position = 0;

        var reloaded = new BasicMemoryVectorDatabase();
        await reloaded.DeserializeFromBinaryStreamAsync(ms);

        Assert.AreEqual(RawFloat32Encoding.EncodingId, reloaded.VectorEncoding.Id);
        Assert.AreEqual(SampleText, reloaded.GetText(1).Text);

        var results = reloaded.Search("Lion King");
        Assert.IsTrue(results.Texts.Any(t => t.Text.Contains("Lion King")));
    }

    [TestMethod]
    public async Task SaveAndLoad_PreservesInt8Encoding()
    {
        var vdb = new BasicMemoryVectorDatabase(Int8ScalarQuantizationEncoding.Instance);
        vdb.AddText(SampleText, "meta");

        using var ms = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(ms);
        ms.Position = 0;

        // Construct the reload-target with raw; the file's encoding should win.
        var reloaded = new BasicMemoryVectorDatabase();
        await reloaded.DeserializeFromBinaryStreamAsync(ms);

        Assert.AreEqual(Int8ScalarQuantizationEncoding.EncodingId, reloaded.VectorEncoding.Id);

        var stored = reloaded.GetText(1);
        Assert.AreEqual(Int8ScalarQuantizationEncoding.EncodingId, stored.EncodedVector.EncodingId);
        Assert.AreEqual(SampleText, stored.Text);
    }

    [TestMethod]
    public async Task RawSavedFile_DoesNotContainEncodingIdField()
    {
        // To preserve byte compatibility with files written by older versions
        // of the library, a raw-encoded database must not emit the new
        // VectorEncodingId property into database.json.
        var vdb = new BasicMemoryVectorDatabase();
        vdb.AddText(SampleText, "meta");

        using var ms = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(ms);
        ms.Position = 0;

        using var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var dbEntry = archive.GetEntry("database.json")!;
        using var dbStream = dbEntry.Open();
        using var reader = new StreamReader(dbStream);
        var json = reader.ReadToEnd();

        StringAssert.Contains(json, "\"ClassType\"");
        Assert.IsFalse(json.Contains("VectorEncodingId"),
            $"Raw-encoded database.json must omit VectorEncodingId. Actual: {json}");
    }

    [TestMethod]
    public async Task Int8SavedFile_RecordsEncodingId()
    {
        var vdb = new BasicMemoryVectorDatabase(Int8ScalarQuantizationEncoding.Instance);
        vdb.AddText(SampleText, "meta");

        using var ms = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(ms);
        ms.Position = 0;

        using var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);
        var dbEntry = archive.GetEntry("database.json")!;
        using var dbStream = dbEntry.Open();
        using var reader = new StreamReader(dbStream);
        var json = reader.ReadToEnd();

        StringAssert.Contains(json, "\"VectorEncodingId\":\"" + Int8ScalarQuantizationEncoding.EncodingId + "\"");
    }
}
