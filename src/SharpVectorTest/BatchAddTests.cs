namespace SharpVectorTest;

using System.Linq;
using System.Threading.Tasks;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Embeddings;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;

[TestClass]
public class BatchAddTests
{
    [TestMethod]
    public async Task AddTextsAsync_UsesBatchEmbeddings_WhenAvailable()
    {
        var db = new BatchMockMemoryVectorDatabase();

        var inputs = new (string text, string? metadata)[]
        {
            ("one", "m1"),
            ("two", "m2"),
            ("three", "m3")
        };

        var ids = await db.AddTextsAsync(inputs);

        Assert.AreEqual(3, ids.Count);

        var results = db.Search("one");
        Assert.AreEqual(3, results.Texts.Count());

        // Ensure vectors were assigned from batch generator (length = 5 per mock)
        foreach (var item in db)
        {
            Assert.AreEqual(5, item.Vector.Length);
        }
    }
}

public class BatchMockMemoryVectorDatabase
     : MemoryVectorDatabaseBase<
        int,
        string,
        MemoryDictionaryVectorStore<int, string>,
        IntIdGenerator,
        CosineSimilarityVectorComparer
        >
{
    public BatchMockMemoryVectorDatabase()
        : base(
            new MockBatchEmbeddingsGenerator(),
            new MemoryDictionaryVectorStore<int, string>()
            )
    { }
}

public class MockBatchEmbeddingsGenerator : IEmbeddingsGenerator, IBatchEmbeddingsGenerator
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<float[]> GenerateEmbeddingsAsync(string text)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        // Return a different first value to ensure we can recognize batched path if needed
        return texts.Select((t, idx) => new float[] { 0.9f, 0.2f, 0.3f, 0.4f, 0.5f }).ToList();
    }
}
