namespace SharpVectorTest;

using System.Diagnostics;
using Build5Nines.SharpVector;

[TestClass]
public class BugDiscoveryTests
{
    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "SharpVectorTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static async Task WaitForDiskCheckpointAsync(string rootPath)
    {
        var indexPath = Path.Combine(rootPath, "index.json");
        var itemsPath = Path.Combine(rootPath, "items.bin");
        var walPath = Path.Combine(rootPath, "wal.log");
        var timeout = Stopwatch.StartNew();

        while (timeout.Elapsed < TimeSpan.FromSeconds(5))
        {
            if (File.Exists(indexPath)
                && File.Exists(itemsPath)
                && new FileInfo(itemsPath).Length > 0
                && File.Exists(walPath)
                && new FileInfo(walPath).Length == 0)
            {
                return;
            }

            await Task.Delay(25);
        }

        Assert.Fail("Timed out waiting for disk checkpoint to complete.");
    }

    [TestMethod]
    public async Task SearchAsync_PageMetadata_ComputesTotalPagesFromTotalCount()
    {
        var vdb = new BasicMemoryVectorDatabase();

        await vdb.AddTextsAsync(
        [
            ("alpha one", "m1"),
            ("alpha two", "m2"),
            ("alpha three", "m3"),
            ("alpha four", "m4"),
            ("alpha five", "m5")
        ]);

        var results = await vdb.SearchAsync("alpha", pageIndex: 0, pageCount: 2);

        Assert.AreEqual(5, results.TotalCount);
        Assert.AreEqual(2, results.Texts.Count());
        Assert.AreEqual(3, results.TotalPages, "TotalPages should be the ceiling of TotalCount / pageCount.");
    }

    [TestMethod]
    public async Task EmptyMemoryDatabase_CanRoundTripThroughBinaryStream()
    {
        var original = new MemoryVectorDatabase<string>();
        await using var stream = new MemoryStream();

        await original.SerializeToBinaryStreamAsync(stream);
        stream.Position = 0;

        var reloaded = new MemoryVectorDatabase<string>();
        await reloaded.DeserializeFromBinaryStreamAsync(stream);

        var id = await reloaded.AddTextAsync("hello world", "meta1");
        var item = reloaded.GetText(id);

        Assert.AreEqual(1, id);
        Assert.AreEqual("hello world", item.Text);
        Assert.AreEqual("meta1", item.Metadata);
    }

    [TestMethod]
    public async Task ReopenedDiskDatabase_GetIds_ShouldIncludePersistedItemsAfterCheckpoint()
    {
        var root = CreateTempDir();
        var db = new BasicDiskVectorDatabase<string>(root);
        var id = await db.AddTextAsync("persisted text", "meta1");

        await WaitForDiskCheckpointAsync(root);

        var reopened = new BasicDiskVectorDatabase<string>(root);

        CollectionAssert.AreEqual(new[] { id }, reopened.GetIds().OrderBy(x => x).ToArray());
        Assert.AreEqual("persisted text", reopened.GetText(id).Text);
    }

    [TestMethod]
    public async Task ReopenedDiskDatabase_SearchAsync_ShouldNotTreatPersistedDatabaseAsEmpty()
    {
        var root = CreateTempDir();
        var db = new BasicDiskVectorDatabase<string>(root);
        var id = await db.AddTextAsync("persisted search text", "meta-search");

        await WaitForDiskCheckpointAsync(root);

        var reopened = new BasicDiskVectorDatabase<string>(root);
        var results = await reopened.SearchAsync("persisted");

        Assert.AreEqual(1, results.TotalCount);
        Assert.AreEqual(id, results.Texts.Single().Id);
        Assert.AreEqual("persisted search text", results.Texts.Single().Text);
        Assert.AreEqual("meta-search", results.Texts.Single().Metadata);
    }
}
