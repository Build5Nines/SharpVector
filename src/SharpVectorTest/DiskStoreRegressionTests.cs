namespace SharpVectorTest;

using System.Text;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

[TestClass]
public class DiskStoreRegressionTests
{
    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "SharpVectorTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [TestMethod]
    public async Task BasicDiskVocabularyStore_DeserializeFromJsonStreamAsync_RestoresReadableCache()
    {
        var root = CreateTempDir();
        var vocabularyStore = new BasicDiskVocabularyStore<string>(root);
        vocabularyStore.Update(["alpha", "beta"]);
        vocabularyStore.Dispose();

        using var stream = new MemoryStream();
        await vocabularyStore.SerializeToJsonStreamAsync(stream);
        stream.Position = 0;

        var reloaded = new BasicDiskVocabularyStore<string>(CreateTempDir());
        await reloaded.DeserializeFromJsonStreamAsync(stream);

        Assert.AreEqual(2, reloaded.Count, "Deserialization should restore in-memory lookup state.");
        Assert.IsTrue(reloaded.TryGetValue("alpha", out var alphaIndex));
        Assert.IsTrue(reloaded.TryGetValue("beta", out var betaIndex));
        Assert.AreEqual(0, alphaIndex);
        Assert.AreEqual(1, betaIndex);
    }

    [TestMethod]
    public async Task BasicDiskVectorStore_SerializeToJsonStreamAsync_PersistsActualItemsForDeserialization()
    {
        var sourceRoot = CreateTempDir();
        using var sourceVocabulary = new BasicDiskVocabularyStore<string>(sourceRoot);
        using var sourceStore = new BasicDiskVectorStore<int, string>(sourceRoot, sourceVocabulary);
        sourceStore.Set(1, new VectorTextItem<string>("alpha text", "m1", new[] { 1f, 2f }));
        await sourceStore.SetAsync(2, new VectorTextItem<string>("beta text", "m2", new[] { 3f, 4f }));

        using var stream = new MemoryStream();
        await sourceStore.SerializeToJsonStreamAsync(stream);
        stream.Position = 0;
        var json = Encoding.UTF8.GetString(stream.ToArray());
        StringAssert.Contains(json, "alpha text");
        StringAssert.Contains(json, "beta text");

        var targetRoot = CreateTempDir();
        using var targetVocabulary = new BasicDiskVocabularyStore<string>(targetRoot);
        using var targetStore = new BasicDiskVectorStore<int, string>(targetRoot, targetVocabulary);
        stream.Position = 0;
        await targetStore.DeserializeFromJsonStreamAsync(stream);

        Assert.AreEqual(2, targetStore.Count);
        Assert.AreEqual("alpha text", targetStore.Get(1).Text);
        Assert.AreEqual("beta text", targetStore.Get(2).Text);
        CollectionAssert.AreEqual(new[] { 1f, 2f }, targetStore.Get(1).Vector);
        CollectionAssert.AreEqual(new[] { 3f, 4f }, targetStore.Get(2).Vector);
    }

    [TestMethod]
    public async Task BasicDiskVectorStore_EnumerationAndDelete_WorkAcrossSyncAndAsyncPaths()
    {
        var root = CreateTempDir();
        using var vocabulary = new BasicDiskVocabularyStore<string>(root);
        using var store = new BasicDiskVectorStore<int, string>(root, vocabulary);
        store.Set(1, new VectorTextItem<string>("alpha", "m1", new[] { 1f }));
        store.Set(2, new VectorTextItem<string>("beta", "m2", new[] { 2f }));

        var syncIds = store.Select(x => x.Key).OrderBy(x => x).ToArray();
        CollectionAssert.AreEqual(new[] { 1, 2 }, syncIds);

        var asyncIds = new List<int>();
        await foreach (var item in store)
        {
            asyncIds.Add(item.Key);
        }
        asyncIds.Sort();
        CollectionAssert.AreEqual(new[] { 1, 2 }, asyncIds);

        Assert.IsTrue(store.ContainsKey(1));
        var removed = store.Delete(1);
        Assert.AreEqual("alpha", removed.Text);
        Assert.IsFalse(store.ContainsKey(1));
        Assert.ThrowsException<KeyNotFoundException>(() => store.Get(1));
    }
}
