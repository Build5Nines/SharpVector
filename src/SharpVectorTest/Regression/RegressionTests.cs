namespace SharpVectorTest.Regression;

using System.Diagnostics;
using System.Threading.Tasks;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

[TestClass]
public class RegressionTests
{
    [TestMethod]
    public void VectorDatabaseVersion_2_0_2_001()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        vdb.LoadFromFile("Regression/regression-vector-database-v2.0.2.b59vdb");

        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task LoadVectorDatabaseInfo_2_0_2_001()
    {
        var file = new FileStream("Regression/regression-vector-database-v2.0.2.b59vdb", FileMode.Open, FileAccess.Read);
        var dbinfo = await DatabaseFile.LoadDatabaseInfoFromZipArchiveAsync(file);

        Assert.AreEqual("Build5Nines.SharpVector", dbinfo.Schema);
        Assert.AreEqual("1.0.0", dbinfo.Version);
        Assert.AreEqual("Build5Nines.SharpVector.MemoryVectorDatabase\u00601[[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", dbinfo.ClassType);
    }
}