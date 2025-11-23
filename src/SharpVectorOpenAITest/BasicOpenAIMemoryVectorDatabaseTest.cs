using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenAI;
using OpenAI.Embeddings;
using Build5Nines.SharpVector.OpenAI;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.IO;
using System;

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

            // Mock the OpenAI EmbeddingClient to return a deterministic embedding vector
            // GenerateEmbeddingAsync(string input, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
            // returns ClientResult<OpenAIEmbedding>. We create one using the Model Factory helpers.
            var embeddingVector = new float[] { 0.1f, 0.2f, 0.3f }; // small deterministic vector for tests
            var openAiEmbedding = OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 0, vector: embeddingVector);
            // Create minimal concrete PipelineResponse implementation to satisfy ClientResult.FromValue without relying on Moq for abstract type
            var response = new TestPipelineResponse();
            var clientResult = ClientResult.FromValue(openAiEmbedding, response);

            _mockEmbeddingClient
                .Setup(c => c.GenerateEmbeddingAsync(
                    It.IsAny<string>(),
                    It.IsAny<EmbeddingGenerationOptions?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResult);

            _database = new BasicOpenAIMemoryVectorDatabase(_mockEmbeddingClient.Object);
        }

        // Minimal headers implementation for TestPipelineResponse
        internal class EmptyPipelineResponseHeaders : PipelineResponseHeaders
        {
            public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() => (new List<KeyValuePair<string,string>>()).GetEnumerator();
            public override bool TryGetValue(string name, out string? value) { value = null; return false; }
            public override bool TryGetValues(string name, out IEnumerable<string>? values) { values = null; return false; }
        }

        // Minimal PipelineResponse implementation
        internal class TestPipelineResponse : PipelineResponse
        {
            private Stream? _contentStream = Stream.Null;
            private readonly EmptyPipelineResponseHeaders _headers = new EmptyPipelineResponseHeaders();
            public override int Status => 200;
            public override string ReasonPhrase => "OK";
            public override Stream? ContentStream { get => _contentStream; set => _contentStream = value; }
            protected override PipelineResponseHeaders HeadersCore => _headers;
            public override BinaryData Content => BinaryData.FromBytes(Array.Empty<byte>());
            public override BinaryData BufferContent(CancellationToken cancellationToken = default) => Content;
            public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(Content);
            public override void Dispose() { _contentStream?.Dispose(); }
        }

        [TestMethod]
        public void TestInitialization()
        {
            Assert.IsNotNull(_database);
        }

        [TestMethod]
        public async Task Test_SaveLoad_01()
        {
            var filename = "openai_test_saveload_01.b59vdb";
#pragma warning disable CS8604 // Possible null reference argument.
            await _database.SaveToFileAsync(filename);
#pragma warning restore CS8604 // Possible null reference argument.

            await _database.LoadFromFileAsync(filename);
        }

        [TestMethod]
        public async Task Test_SaveLoad_TestIds_01()
        {
            _database.AddText("Sample text for testing IDs.", "111");
            _database.AddText("Another sample text for testing IDs.", "222");

            var results = _database.Search("testing IDs");
            Assert.AreEqual(2, results.Texts.Count());

            var filename = "openai_test_saveload_testids_01.b59vdb";
#pragma warning disable CS8604 // Possible null reference argument.
            await _database.SaveToFileAsync(filename);
#pragma warning restore CS8604 // Possible null reference argument.

            await _database.LoadFromFileAsync(filename);

            _database.AddText("A new text after loading to check ID assignment.", "333");

            var newResults = _database.Search("testing IDs");
            Assert.AreEqual(3, newResults.Texts.Count());
            var texts = newResults.Texts.OrderBy(x => x.Metadata).ToArray();
            Assert.AreEqual("111", texts[0].Metadata);
            Assert.AreEqual("222", texts[1].Metadata);
            Assert.AreEqual("333", texts[2].Metadata);
        }

        [TestMethod]
        public async Task Test_SaveLoad_TestIds_02()
        {
            _database.AddText("Sample text for testing IDs.", "111");
            _database.AddText("Another sample text for testing IDs.", "222");

            var results = _database.Search("testing IDs");
            Assert.AreEqual(2, results.Texts.Count());

            var filename = "openai_test_saveload_testids_02.b59vdb";
#pragma warning disable CS8604 // Possible null reference argument.
            await _database.SaveToFileAsync(filename);
#pragma warning restore CS8604 // Possible null reference argument.

            var newdb = new BasicOpenAIMemoryVectorDatabase(_mockEmbeddingClient.Object);
            await newdb.LoadFromFileAsync(filename);
    
            newdb.AddText("A new text after loading to check ID assignment.", "333");

            var newResults = newdb.Search("testing IDs");
            Assert.AreEqual(3, newResults.Texts.Count());
            var texts = newResults.Texts.OrderBy(x => x.Metadata).ToArray();
            Assert.AreEqual("111", texts[0].Metadata);
            Assert.AreEqual("222", texts[1].Metadata);
            Assert.AreEqual("333", texts[2].Metadata);
        }
    }
}