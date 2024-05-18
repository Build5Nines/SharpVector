namespace SharpVectorTest;

using Build5Nines.SharpVector;

[TestClass]
public class VectorDatabaseTest
{
    [TestMethod]
    public void SimpleTest()
    {
        IVectorDatabase<double> vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));
        Assert.AreEqual(0.3396831154823303, results.Texts[0].Similarity);
    }
}