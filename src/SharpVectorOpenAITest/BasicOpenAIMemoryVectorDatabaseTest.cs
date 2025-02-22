using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenAI;
using OpenAI.Embeddings;
using Build5Nines.SharpVector.OpenAI;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Build5Nines.SharpVector.OpenAI.Tests
{
    [TestClass]
    public class BasicMemoryVectorDatabaseTest
    {
        private Mock<EmbeddingClient>? _mockEmbeddingClient;
        private BasicOpenAIMemoryVectorDatabase? _database;

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

    }
}