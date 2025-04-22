using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;

namespace Build5Nines.SharpVector;

public interface IMemoryVectorDatabase<TId, TMetadata> : IVectorDatabase<TId, TMetadata>
    where TId : notnull
{ }

/// <summary>
/// A simple in-memory database for storing and querying vectorized text items.
/// This database uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.
/// </summary>
/// <typeparam name="TMetadata">Defines the data type for the Metadata stored with the Text.</typeparam>
public class MemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int,
        TMetadata,
        MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>,
        DictionaryVocabularyStore<string>,
        string, int,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizer<string, int>,
        CosineSimilarityVectorComparer
        >, IMemoryVectorDatabase<int, TMetadata>, IVectorDatabase<int, TMetadata>
{
    public MemoryVectorDatabase()
        : base(
            new MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>(
                new DictionaryVocabularyStore<string>()
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

    /// <summary>
    /// Deserializes the database from a binary stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public override async Task DeserializeFromBinaryStreamAsync(Stream stream)
    {
        await base.DeserializeFromBinaryStreamAsync(stream);

        // Re-initialize the IdGenerator with the max Id value from the VectorStore
        _idGenerator = new IntIdGenerator(VectorStore.GetIds().Max());
    }

    /// <summary>
    /// Deserializes the database from a binary stream.
    /// </summary>
    /// <param name="stream"></param>
    public override void DeserializeFromBinaryStream(Stream stream)
    {
        base.DeserializeFromBinaryStream(stream);

        // Re-initialize the IdGenerator with the max Id value from the VectorStore
        _idGenerator = new IntIdGenerator(VectorStore.GetIds().Max());
    }
}
