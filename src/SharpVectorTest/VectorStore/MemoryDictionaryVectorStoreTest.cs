using Build5Nines.SharpVector;
using Build5Nines.SharpVector.VectorStore;

namespace SharpVectorTest.VectorStore;

[TestClass]
public class MemoryDictionaryVectorStoreTests
{
    [TestMethod]
    public async Task SerializeDeserializeStream_001()
    {
        var vectorStore = new MemoryDictionaryVectorStore<int, string>();
        vectorStore.Set(1, new VectorTextItem<string, string>("key1", "1", new float[] { 1.0F, 2.0F, 3.0F }));
        vectorStore.Set(2, new VectorTextItem<string, string>("key2", "2", new float[] { 4.0F, 5.0F, 6.0F }));
        vectorStore.Set(3, new VectorTextItem<string, string>("key3", "3", new float[] { 7.0F, 8.0F, 9.0F }));
        vectorStore.Set(4, new VectorTextItem<string, string>("key4", "4", new float[] { 10.0F, 11.0F, 12.0F }));


        var stream = new MemoryStream();
        await vectorStore.SerializeToJsonStreamAsync(stream);

        stream.Position = 0; // move to beginning of stream

        var vectorStoreTwo = new MemoryDictionaryVectorStore<int, string>();
        await vectorStoreTwo.DeserializeFromJsonStreamAsync(stream);

        Assert.AreEqual(4, vectorStoreTwo.Count());
        
        Assert.AreEqual(3, vectorStoreTwo.Get(1).Vector.Length);
        Assert.AreEqual(3, vectorStoreTwo.Get(2).Vector.Length);
        Assert.AreEqual(3, vectorStoreTwo.Get(3).Vector.Length);
        Assert.AreEqual(3, vectorStoreTwo.Get(4).Vector.Length);
        
        Assert.AreEqual(1.0, vectorStoreTwo.Get(1).Vector[0]);
        Assert.AreEqual(2.0, vectorStoreTwo.Get(1).Vector[1]);
        Assert.AreEqual(3.0, vectorStoreTwo.Get(1).Vector[2]);

        Assert.AreEqual(4.0, vectorStoreTwo.Get(2).Vector[0]);
        Assert.AreEqual(5.0, vectorStoreTwo.Get(2).Vector[1]);
        Assert.AreEqual(6.0, vectorStoreTwo.Get(2).Vector[2]);
        
        Assert.AreEqual(7.0, vectorStoreTwo.Get(3).Vector[0]);
        Assert.AreEqual(8.0, vectorStoreTwo.Get(3).Vector[1]);
        Assert.AreEqual(9.0, vectorStoreTwo.Get(3).Vector[2]);

        Assert.AreEqual(10.0, vectorStoreTwo.Get(4).Vector[0]);
        Assert.AreEqual(11.0, vectorStoreTwo.Get(4).Vector[1]);
        Assert.AreEqual(12.0, vectorStoreTwo.Get(4).Vector[2]);
    }
}