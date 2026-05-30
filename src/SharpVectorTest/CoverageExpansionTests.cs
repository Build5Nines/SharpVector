namespace SharpVectorTest;

using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Text;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Data;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

[TestClass]
public class CoverageExpansionTests
{
    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "SharpVectorTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static MemoryStream CreateZipArchive(params (string name, string content)[] entries)
    {
        var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var entry in entries)
            {
                var zipEntry = archive.CreateEntry(entry.name);
                using var entryStream = zipEntry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8, leaveOpen: true);
                writer.Write(entry.content);
            }
        }

        stream.Position = 0;
        return stream;
    }

    [TestMethod]
    public void DatabaseInfo_Constructors_AssignExpectedValues()
    {
        var empty = new DatabaseInfo();
        Assert.IsNull(empty.Schema);
        Assert.IsNull(empty.Version);
        Assert.IsNull(empty.ClassType);

        var classOnly = new DatabaseInfo("MyType");
        Assert.AreEqual("Build5Nines.SharpVector", classOnly.Schema);
        Assert.AreEqual("1.0.0", classOnly.Version);
        Assert.AreEqual("MyType", classOnly.ClassType);

        var custom = new DatabaseInfo("schema", "2.0", "CustomType");
        Assert.AreEqual("schema", custom.Schema);
        Assert.AreEqual("2.0", custom.Version);
        Assert.AreEqual("CustomType", custom.ClassType);
    }

    [TestMethod]
    public void DatabaseFileExceptions_PreserveMessagesAndState()
    {
        var inner = new InvalidOperationException("inner");

        var baseDefault = new DatabaseFileException();
        Assert.IsNotNull(baseDefault);

        var baseWithMessage = new DatabaseFileException("base message");
        Assert.AreEqual("base message", baseWithMessage.Message);

        var baseWithInner = new DatabaseFileException("wrapped", inner);
        Assert.AreEqual("wrapped", baseWithInner.Message);
        Assert.AreSame(inner, baseWithInner.InnerException);

        Assert.AreEqual("info", new DatabaseFileInfoException("info").Message);
        Assert.AreEqual("schema", new DatabaseFileSchemaException("schema").Message);
        Assert.AreEqual("version", new DatabaseFileVersionException("version").Message);
        Assert.AreEqual("class", new DatabaseFileClassTypeException("class").Message);

        var missing = new DatabaseFileMissingEntryException("missing", "vectorstore");
        Assert.AreEqual("missing", missing.Message);
        Assert.AreEqual("vectorstore", missing.MissingEntry);
    }

    [TestMethod]
    public void VectorTextModelTypes_ExposeExpectedProperties()
    {
        var vector = new[] { 1f, 2f, 3f };
        var databaseItem = new VectorTextDatabaseItem<int, string, string>(7, "hello", "meta", vector);
        Assert.AreEqual(7, databaseItem.Id);
        Assert.AreEqual("hello", databaseItem.Text);
        Assert.AreEqual("meta", databaseItem.Metadata);
        CollectionAssert.AreEqual(vector, databaseItem.Vector);

        var textItem = new VectorTextItem<string>("doc", "m1", vector);
        textItem.Text = "doc2";
        textItem.Metadata = "m2";
        textItem.Vector = new[] { 5f, 6f };
        Assert.AreEqual("doc2", textItem.Text);
        Assert.AreEqual("m2", textItem.Metadata);
        CollectionAssert.AreEqual(new[] { 5f, 6f }, textItem.Vector);

        var resultItem = new VectorTextResultItem<string>(9, textItem, 0.42f);
        Assert.AreEqual(9, resultItem.Id);
        Assert.AreEqual("doc2", resultItem.Text);
        Assert.AreEqual("m2", resultItem.Metadata);
        Assert.AreEqual(0.42f, resultItem.Similarity);
        CollectionAssert.AreEqual(new[] { 5f, 6f }, resultItem.Vectors.ToArray());
#pragma warning disable CS0618
        Assert.AreEqual(0.42f, resultItem.VectorComparison);
#pragma warning restore CS0618
    }

    [TestMethod]
    public void VectorTextResult_IsEmpty_TracksNullEmptyAndPopulatedCollections()
    {
        var nullTexts = new VectorTextResult<int, string, string>(0, 0, 0, null!);
        Assert.IsTrue(nullTexts.IsEmpty);

        var emptyTexts = new VectorTextResult<string>(0, 0, 0, Array.Empty<IVectorTextResultItem<int, string, string>>());
        Assert.IsTrue(emptyTexts.IsEmpty);
        Assert.AreEqual(0, emptyTexts.TotalCount);
        Assert.AreEqual(0, emptyTexts.PageIndex);
        Assert.AreEqual(0, emptyTexts.TotalPages);

        var item = new VectorTextResultItem<string>(1, new VectorTextItem<string>("alpha", "meta", new[] { 1f }), 0.9f);
        var populated = new VectorTextResult<string>(1, 2, 3, new[] { item });
        Assert.IsFalse(populated.IsEmpty);
        Assert.AreEqual(1, populated.TotalCount);
        Assert.AreEqual(2, populated.PageIndex);
        Assert.AreEqual(3, populated.TotalPages);
    }

    [TestMethod]
    public void IdGenerators_GenerateExpectedIdentifiers()
    {
        var guidGenerator = new GuidIdGenerator();
        var firstGuid = guidGenerator.NewId();
        var secondGuid = guidGenerator.NewId();
        Assert.AreNotEqual(Guid.Empty, firstGuid);
        Assert.AreNotEqual(firstGuid, secondGuid);

        var intGenerator = new IntIdGenerator();
        Assert.AreEqual(1, intGenerator.NewId());
        Assert.AreEqual(2, intGenerator.NewId());

        var seededIntGenerator = new IntIdGenerator(10);
        Assert.AreEqual(11, seededIntGenerator.NewId());

        var numericGenerator = new NumericIdGenerator<long>(100);
        Assert.AreEqual(101L, numericGenerator.NewId());
        numericGenerator.SetMostRecent(500L);
        Assert.AreEqual(501L, numericGenerator.NewId());
    }

    [TestMethod]
    public async Task MemoryAndVocabularyStores_HandleNullJsonDeleteAndAsyncEnumeration()
    {
        var vectorStore = new MemoryDictionaryVectorStore<int, string>();
        await vectorStore.SetAsync(1, new VectorTextItem<string>("first", "m1", new[] { 1f, 2f }));
        Assert.IsTrue(vectorStore.ContainsKey(1));
        Assert.AreEqual("first", vectorStore.Get(1).Text);

        var seen = new List<int>();
        await foreach (var item in vectorStore)
        {
            seen.Add(item.Key);
        }
        CollectionAssert.AreEqual(new[] { 1 }, seen);

        var removed = vectorStore.Delete(1);
        Assert.AreEqual("first", removed.Text);
        Assert.IsFalse(vectorStore.ContainsKey(1));
        Assert.ThrowsException<KeyNotFoundException>(() => vectorStore.Get(1));
        Assert.ThrowsException<KeyNotFoundException>(() => vectorStore.Delete(1));
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => vectorStore.SerializeToJsonStreamAsync(null!));

        using var nullVectorStoreStream = new MemoryStream(Encoding.UTF8.GetBytes("null"));
        await vectorStore.DeserializeFromJsonStreamAsync(nullVectorStoreStream);
        Assert.AreEqual(0, vectorStore.Count);

        var vocabularyStore = new DictionaryVocabularyStore<string>();
        vocabularyStore.Update(["alpha", "beta"]);
        Assert.AreEqual(2, vocabularyStore.Count);
        Assert.IsTrue(vocabularyStore.TryGetValue("alpha", out _));
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => vocabularyStore.SerializeToJsonStreamAsync(null!));

        using var nullVocabularyStream = new MemoryStream(Encoding.UTF8.GetBytes("null"));
        await vocabularyStore.DeserializeFromJsonStreamAsync(nullVocabularyStream);
        Assert.AreEqual(0, vocabularyStore.Count);
        Assert.IsFalse(vocabularyStore.TryGetValue("alpha", out _));
    }

    [TestMethod]
    public async Task TextDataLoader_CoversChunkingAndValidationBranches()
    {
        var loader = new InspectableTextDataLoader(new BasicMemoryVectorDatabase());

        var sentences = loader.ExposeChunkText(
            "One. Two? Three!",
            new TextChunkingOptions<string>
            {
                Method = TextChunkingMethod.Sentence,
                RetrieveMetadata = chunk => chunk
            });
        CollectionAssert.AreEqual(new[] { "One.", "Two?", "Three!" }, sentences);

        var fixedLength = loader.ExposeChunkText(
            "Hello world from SharpVector tests",
            new TextChunkingOptions<string>
            {
                Method = TextChunkingMethod.FixedLength,
                ChunkSize = 2,
                RetrieveMetadata = chunk => chunk
            });
        CollectionAssert.AreEqual(new[] { "hello world", "from sharpvector", "tests" }, fixedLength);

        var chineseAndEnglish = loader.ExposeChunkText(
            "你好世界 alpha beta",
            new TextChunkingOptions<string>
            {
                Method = TextChunkingMethod.FixedLength,
                ChunkSize = 6,
                RetrieveMetadata = chunk => chunk
            });
        CollectionAssert.AreEqual(new[] { "你好世界 alpha beta" }, chineseAndEnglish);

        Assert.ThrowsException<ArgumentException>(() =>
            loader.ExposeChunkText(
                "one two three",
                new TextChunkingOptions<string>
                {
                    Method = (TextChunkingMethod)999,
                    RetrieveMetadata = chunk => chunk
                }));

        Assert.ThrowsException<ArgumentException>(() =>
            loader.ExposeChunkText(
                "one two three four",
                new TextChunkingOptions<string>
                {
                    Method = TextChunkingMethod.OverlappingWindow,
                    ChunkSize = 2,
                    OverlapSize = 2,
                    RetrieveMetadata = chunk => chunk
                }));

        var syncLoader = new TextDataLoader<int, string>(new BasicMemoryVectorDatabase());
        Assert.ThrowsException<ValidationException>(() =>
            syncLoader.AddDocument(
                "document",
                new TextChunkingOptions<string>
                {
                    Method = TextChunkingMethod.Paragraph,
                    RetrieveMetadata = null!
                }));

        await Assert.ThrowsExceptionAsync<ValidationException>(() =>
            syncLoader.AddDocumentAsync(
                "document",
                new TextChunkingOptions<string>
                {
                    Method = TextChunkingMethod.Paragraph,
                    RetrieveMetadata = null!
                }));
    }

    [TestMethod]
    public async Task DatabaseFile_HelperMethods_ValidateStreamsEntriesAndMetadata()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            DatabaseFile.SaveDatabaseToZipArchiveAsync(null!, new DatabaseInfo("Type"), _ => Task.CompletedTask));

        using var nullInfoStream = new MemoryStream(Encoding.UTF8.GetBytes("null"));
        await Assert.ThrowsExceptionAsync<DatabaseFileInfoException>(() => DatabaseFile.LoadDatabaseInfoFromJsonAsync(nullInfoStream));

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            DatabaseFile.LoadDatabaseFromZipArchiveAsync(null!, "Type", _ => Task.CompletedTask));

        using (var missingDatabaseArchive = CreateZipArchive())
        using (var archive = new ZipArchive(missingDatabaseArchive, ZipArchiveMode.Read, leaveOpen: true))
        {
            await Assert.ThrowsExceptionAsync<DatabaseFileMissingEntryException>(() => DatabaseFile.LoadDatabaseInfoAsync(archive));
        }

        using (var missingVectorArchiveStream = CreateZipArchive(("database.json", "{}")))
        using (var archive = new ZipArchive(missingVectorArchiveStream, ZipArchiveMode.Read, leaveOpen: true))
        {
            var vectorStore = new MemoryDictionaryVectorStore<int, string>();
            var ex = await Assert.ThrowsExceptionAsync<DatabaseFileMissingEntryException>(() => DatabaseFile.LoadVectorStoreAsync(archive, vectorStore));
            Assert.AreEqual("vectorstore", ex.MissingEntry);
        }

        using (var missingVocabularyArchiveStream = CreateZipArchive(("database.json", "{}")))
        using (var archive = new ZipArchive(missingVocabularyArchiveStream, ZipArchiveMode.Read, leaveOpen: true))
        {
            var vocabularyStore = new DictionaryVocabularyStore<string>();
            var ex = await Assert.ThrowsExceptionAsync<DatabaseFileMissingEntryException>(() => DatabaseFile.LoadVocabularyStoreAsync(archive, vocabularyStore));
            Assert.AreEqual("vocabularystore", ex.MissingEntry);
        }

        using var schemaStream = CreateZipArchive(("database.json", "{\"Schema\":\"Wrong\",\"Version\":\"1.0.0\",\"ClassType\":\"Type\"}"));
        await Assert.ThrowsExceptionAsync<DatabaseFileSchemaException>(() =>
            DatabaseFile.LoadDatabaseFromZipArchiveAsync(schemaStream, "Type", _ => Task.CompletedTask));

        using var versionStream = CreateZipArchive(("database.json", "{\"Schema\":\"Build5Nines.SharpVector\",\"Version\":\"2.0.0\",\"ClassType\":\"Type\"}"));
        await Assert.ThrowsExceptionAsync<DatabaseFileVersionException>(() =>
            DatabaseFile.LoadDatabaseFromZipArchiveAsync(versionStream, "Type", _ => Task.CompletedTask));

        using var classTypeStream = CreateZipArchive(("database.json", "{\"Schema\":\"Build5Nines.SharpVector\",\"Version\":\"1.0.0\",\"ClassType\":\"OtherType\"}"));
        await Assert.ThrowsExceptionAsync<DatabaseFileClassTypeException>(() =>
            DatabaseFile.LoadDatabaseFromZipArchiveAsync(classTypeStream, "Type", _ => Task.CompletedTask));
    }

    [TestMethod]
    public async Task VectorDatabaseBase_BranchesCoverNullEmptyAndEmptyDatabasePaths()
    {
        var bowDatabase = new BasicMemoryVectorDatabase();
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => bowDatabase.AddTextsAsync(null!));
        Assert.AreEqual(0, (await bowDatabase.AddTextsAsync(Array.Empty<(string text, string? metadata)>())).Count);
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => bowDatabase.SearchAsync("query"));

        var embeddingsDatabase = new EmbeddingGeneratorMemoryVectorDatabase();
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => embeddingsDatabase.AddTextsAsync(null!));
        Assert.AreEqual(0, (await embeddingsDatabase.AddTextsAsync(Array.Empty<(string text, string? metadata)>())).Count);
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => embeddingsDatabase.SearchAsync("query"));
    }

    [TestMethod]
    public void MemoryVectorDatabase_ObsoleteSyncSerializationMethods_RoundTripEmptyAndNonEmptyDatabases()
    {
#pragma warning disable CS0618
        var populated = new MemoryVectorDatabase<string>();
        populated.AddText("hello world", "m1");
        using var populatedStream = new MemoryStream();
        populated.SerializeToJsonStream(populatedStream);
        populatedStream.Position = 0;

        var reloaded = new MemoryVectorDatabase<string>();
        reloaded.DeserializeFromJsonStream(populatedStream);
        Assert.AreEqual("hello world", reloaded.GetText(1).Text);
        Assert.AreEqual("m1", reloaded.GetText(1).Metadata);

        var empty = new MemoryVectorDatabase<string>();
        using var emptyStream = new MemoryStream();
        empty.SerializeToJsonStream(emptyStream);
        emptyStream.Position = 0;

        var reloadedEmpty = new MemoryVectorDatabase<string>();
        reloadedEmpty.DeserializeFromJsonStream(emptyStream);
        var id = reloadedEmpty.AddText("new item", "m2");
        Assert.AreEqual(1, id);
#pragma warning restore CS0618
    }

    [TestMethod]
    public async Task MemoryVectorDatabase_ObsoleteAsyncSerializationMethods_RoundTripDatabase()
    {
#pragma warning disable CS0618
        var database = new MemoryVectorDatabase<string>();
        await database.AddTextAsync("alpha", "m1");
        using var stream = new MemoryStream();
        await database.SerializeToJsonStreamAsync(stream);
        stream.Position = 0;

        var reloaded = new MemoryVectorDatabase<string>();
        await reloaded.DeserializeFromJsonStreamAsync(stream);
        Assert.AreEqual("alpha", reloaded.GetText(1).Text);
        Assert.AreEqual("m1", reloaded.GetText(1).Metadata);
#pragma warning restore CS0618
    }

    [TestMethod]
    public async Task EmbeddingVectorDatabase_ObsoleteSerializationMethods_RoundTripDatabase()
    {
#pragma warning disable CS0618
        var database = new EmbeddingGeneratorMemoryVectorDatabase();
        await database.AddTextAsync("alpha", "m1");

        using var asyncStream = new MemoryStream();
        await database.SerializeToJsonStreamAsync(asyncStream);
        asyncStream.Position = 0;
        var asyncReloaded = new EmbeddingGeneratorMemoryVectorDatabase();
        await asyncReloaded.DeserializeFromJsonStreamAsync(asyncStream);
        Assert.AreEqual("alpha", asyncReloaded.GetText(1).Text);

        using var syncStream = new MemoryStream();
        database.SerializeToJsonStream(syncStream);
        syncStream.Position = 0;
        var syncReloaded = new EmbeddingGeneratorMemoryVectorDatabase();
        syncReloaded.DeserializeFromJsonStream(syncStream);
        Assert.AreEqual("alpha", syncReloaded.GetText(1).Text);
#pragma warning restore CS0618
    }

    [TestMethod]
    public async Task BasicDiskVectorDatabase_ObsoleteDeserializeMethods_RoundTripEmptyDatabase()
    {
#pragma warning disable CS0618
        var source = new BasicDiskVectorDatabase<string>(CreateTempDir());

        using var asyncStream = new MemoryStream();
        await source.SerializeToJsonStreamAsync(asyncStream);
        asyncStream.Position = 0;

        var asyncReloaded = new BasicDiskVectorDatabase<string>(CreateTempDir());
        await asyncReloaded.DeserializeFromJsonStreamAsync(asyncStream);
        var asyncId = await asyncReloaded.AddTextAsync("alpha", "m1");
        Assert.AreEqual(1, asyncId);

        using var syncStream = new MemoryStream();
        source.SerializeToJsonStream(syncStream);
        syncStream.Position = 0;

        var syncReloaded = new BasicDiskVectorDatabase<string>(CreateTempDir());
        syncReloaded.DeserializeFromJsonStream(syncStream);
        var syncId = syncReloaded.AddText("beta", "m2");
        Assert.AreEqual(1, syncId);
#pragma warning restore CS0618
    }

    private sealed class InspectableTextDataLoader : TextDataLoader<int, string>
    {
        public InspectableTextDataLoader(IVectorDatabase<int, string> vectorDatabase)
            : base(vectorDatabase)
        {
        }

        public List<string> ExposeChunkText(string text, TextChunkingOptions<string> chunkingOptions)
        {
            return base.ChunkText(text, chunkingOptions);
        }
    }
}
