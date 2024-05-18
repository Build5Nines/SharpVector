namespace SharpVectorTest;

using Build5Nines.SharpVector;

[TestClass]
public class VectorDatabaseTest
{
    private float similarityEqualsTolerance = 1e-8f;

    [TestMethod]
    public void SimpleTest()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts[0].Metadata);
        Assert.AreEqual("0.3396831154823303", results.Texts[0].Similarity.ToString());
    }

    [TestMethod]
    public void SimpleTest_IMemoryVectorDatabase()
    {
        IVectorDatabase<int, double> vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts[0].Metadata);
        Assert.AreEqual(0.3396831154823303f, results.Texts[0].Similarity, similarityEqualsTolerance);
    }

    [TestMethod]
    public void Text_Update_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));

        vdb.UpdateText(id, "The Lion King is a great movie!");

        results = vdb.Search("Lion King");
        Assert.AreEqual("The Lion King is a great movie!", results.Texts[0].Text);
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts[0].Metadata);
    }

    [TestMethod]
    public void Text_Metadata_String_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts[0].Metadata);
        Assert.AreEqual(0.3396831154823303f, results.Texts[0].Similarity, similarityEqualsTolerance);
    }

    [TestMethod]
    public void Text_Metadata_String_Update()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Length);
        Assert.IsTrue(results.Texts[0].Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts[0].Metadata);
        Assert.AreEqual(0.3396831154823303f, results.Texts[0].Similarity, similarityEqualsTolerance);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts[0].Metadata);
    }
}