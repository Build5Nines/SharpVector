using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;

namespace Build5Nines.SharpVector;

/// <summary>
/// A basic disk-backed vector database using Bag-of-Words, Cosine similarity,
/// disk-backed vector store and vocabulary store. Uses int IDs and string metadata.
/// </summary>
public class BasicDiskVectorDatabase<TMetadata>
     : BasicDiskMemoryVectorDatabaseBase<
        int,
        TMetadata,
        BasicDiskVectorStore<int, TMetadata, BasicDiskVocabularyStore<string>, string, int>,
        BasicDiskVocabularyStore<string>,
        string, int,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizer<string, int>,
        CosineSimilarityVectorComparer
        >, IMemoryVectorDatabase<int, TMetadata>, IVectorDatabase<int, TMetadata>
{
    public BasicDiskVectorDatabase(string rootPath)
        : base(
            new BasicDiskVectorStore<int, TMetadata, BasicDiskVocabularyStore<string>, string, int>(
                rootPath,
                new BasicDiskVocabularyStore<string>(rootPath)
            )
        )
    { }

    [Obsolete("Use DeserializeFromBinaryStreamAsync instead.")]
    public override async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        await DeserializeFromBinaryStreamAsync(stream);
    }

    [Obsolete("Use DeserializeFromBinaryStream instead.")]
    public override void DeserializeFromJsonStream(Stream stream)
    {
        DeserializeFromBinaryStream(stream);
    }
}
