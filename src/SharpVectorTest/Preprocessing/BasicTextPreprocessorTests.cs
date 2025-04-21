namespace SharpVectorTest.Preprocessing;

using System.Diagnostics;
using System.Threading.Tasks;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Embeddings;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

[TestClass]
public class VectorDatabaseTests
{
    [TestMethod]
    public void TokenizeAndPreprocess_Null()
    {
        var preprocessor = new BasicTextPreprocessor();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var tokens = preprocessor.TokenizeAndPreprocess(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        Assert.AreEqual(0, tokens.Count());
    }

    [TestMethod]
    public void TokenizeAndPreprocess_Empty()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess(string.Empty);
        
        Assert.AreEqual(0, tokens.Count());
    }

    [TestMethod]
    public void TokenizeAndPreprocess_Whitespace()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess(" ");
        
        Assert.AreEqual(0, tokens.Count());
    }

    [TestMethod]
    public void TokenizeAndPreprocess_Punctuation_01()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello.!@#$%^&*()`~世-_=+ 界{}[]|:;\"',.<>/?!");
        
        var expectedTokens = new List<string> { "hello", "世", "界"};
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }

    [TestMethod]
    public void TokenizeAndPreprocess_Punctuation_02()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello.!@#$%^&*()`~-_=+{}[]|:;\"',.<>/?");
        
        var expectedTokens = new List<string> { "hello" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }

    [TestMethod]
    public void TokenizeAndPreprocess_Punctuation_03()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello.🔥!@#$%^&*()`~世-_=+ 界{}[]|:;\"',.<>/?");
        
        var expectedTokens = new List<string> { "hello", "🔥", "世", "界"};
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }
    
    [TestMethod]
    public void TokenizeAndPreprocess_Punctuation_04()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello.!@#🔥$%^&*()`~-_=+{}[]|:;\"',.<>/?");
        
        var expectedTokens = new List<string> { "hello", "🔥" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }

    [TestMethod]
    public void TokenizeAndPreprocess_01()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello, world! 你好，世界！");
        
        var expectedTokens = new List<string> { "hello", "world", "你", "好", "世", "界" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }

    [TestMethod]
    public void TokenizeAndPreprocess_02()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello, World! How are you?");
        
        var expectedTokens = new List<string> { "hello", "world", "how", "are", "you" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match");
        }
    }  

    [TestMethod]
    public void TokenizeAndPreprocess_03()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello, World! 👑🔥 How are you? 🔥.");
        
        var expectedTokens = new List<string> { "hello", "world", "👑", "🔥", "how", "are", "you", "🔥" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match ::" + String.Join("-", tokens));
        }
    } 

    [TestMethod]
    public void TokenizeAndPreprocess_04()
    {
        var preprocessor = new BasicTextPreprocessor();
        var tokens = preprocessor.TokenizeAndPreprocess("Hello, world! 👑🔥你好，世界！👑 ");
        
        var expectedTokens = new List<string> { "hello", "world", "👑", "🔥", "你", "好", "世", "界", "👑" };
        for(var i = 0; i < expectedTokens.Count; i++)
        {
            Assert.AreEqual(expectedTokens[i], tokens.ElementAt(i), $"Index: {i} does not match ::" + String.Join("-", tokens));
        }
    }
}