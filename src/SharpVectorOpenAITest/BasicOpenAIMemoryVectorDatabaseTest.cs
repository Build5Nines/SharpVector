using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenAI.Embeddings;
using Build5Nines.SharpVector.OpenAI;
using System.Threading.Tasks;
using System.ClientModel;

namespace Build5Nines.SharpVector.OpenAI.Tests
{
    [TestClass]
    public class BasicMemoryVectorDatabaseTest
    {
        private Mock<EmbeddingClient> _mockEmbeddingClient;
        private BasicOpenAIMemoryVectorDatabase _database;

        [TestInitialize]
        public void Setup()
        {
            _mockEmbeddingClient = new Mock<EmbeddingClient>();
            _database = new BasicOpenAIMemoryVectorDatabase(_mockEmbeddingClient.Object);
        }

        [TestMethod]
        public void TestInitialization()
        {
            Assert.IsNotNull(_database);
        }

        // [TestMethod]
        // public async Task TestGetVectors()
        // {
        //     // Arrange
        //     var text = "sample text";
        //     var expectedVector = new float[] { 0.1f, 0.2f, 0.3f };
        //     _mockEmbeddingClient
        //         .Setup(client => client.GenerateEmbeddingAsync(It.Is<string>(s => s.Equals(text)), It.IsAny<EmbeddingGenerationOptions>()))
        //         .ReturnsAsync(new ClientResult<OpenAIEmbedding> { Data = expectedVector });

        //     // Act
        //     var result = await _database.GetVectors(text);

        //     // Assert
        //     CollectionAssert.AreEqual(expectedVector, result);
        // }

        // [TestMethod]
        // public async Task TestAddVector()
        // {
        //     // Arrange
        //     var text = "sample text";
        //     var vector = new float[] { 0.1f, 0.2f, 0.3f };
        //     _mockEmbeddingClient
        //         .Setup(client => client.GenerateEmbeddingAsync(It.Is<string>(s => s == text), It.IsAny<EmbeddingGenerationOptions>()))
        //         .ReturnsAsync(new ClientResult<OpenAIEmbedding> { Data = vector });

        //     // Act
        //     await _database.AddVector(text);

        //     // Assert
        //     var storedVector = await _database.GetVectors(text);
        //     CollectionAssert.AreEqual(vector, storedVector);
        // }
    }
}