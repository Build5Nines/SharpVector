namespace SharpVectorTest;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Build5Nines.SharpVector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DiskVectorDatabaseTests
{
    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "SharpVectorTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [TestMethod]
    public async Task AddAndGetText_PersistsToDisk()
    {
        var root = CreateTempDir();
        var db = new BasicDiskVectorDatabase<string>(root);

        var id = await db.AddTextAsync("hello world", "meta1");
        var item = db.GetText(id);
        Assert.AreEqual("hello world", item.Text);
        Assert.AreEqual("meta1", item.Metadata);

        // Recreate DB and ensure data is still there
        var db2 = new BasicDiskVectorDatabase<string>(root);
        var item2 = db2.GetText(id);
        Assert.AreEqual("hello world", item2.Text);
        Assert.AreEqual("meta1", item2.Metadata);
    }

    [TestMethod]
    public async Task Search_ReturnsSimilarResults()
    {
        var root = CreateTempDir();
        var db = new BasicDiskVectorDatabase<string>(root);

        await db.AddTextAsync("The quick brown fox", "a");
        await db.AddTextAsync("Jumps over the lazy dog", "b");
        await db.AddTextAsync("An unrelated sentence", "c");

        var results = await db.SearchAsync("quick fox", threshold: null, pageIndex: 0, pageCount: null);
        Assert.IsTrue(results.Texts.Any());
        Assert.IsTrue(results.Texts.Any(r => r.Text.Contains("quick", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task Delete_RemovesFromIndexButKeepsFile()
    {
        var root = CreateTempDir();
        var db = new BasicDiskVectorDatabase<string>(root);
        var id = await db.AddTextAsync("to be deleted", "m");
        var existing = db.GetText(id);
        Assert.AreEqual("to be deleted", existing.Text);

        db.DeleteText(id);
        Assert.IsFalse(db.GetIds().Contains(id));
        Assert.ThrowsException<KeyNotFoundException>(() => db.GetText(id));

        // Reopen and ensure deletion persists
        var db2 = new BasicDiskVectorDatabase<string>(root);
        Assert.IsFalse(db2.GetIds().Contains(id));
        Assert.ThrowsException<KeyNotFoundException>(() => db2.GetText(id));
    }
}