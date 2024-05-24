using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;

namespace Build5Nines.SharpVector;

/// <summary>
/// A simple in-memory database for storing and querying vectorized text items.
/// This database uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.
/// </summary>
/// <typeparam name="TMetadata">Defines the data type for the Metadata stored with the Text.</typeparam>
public class MemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int, 
        TMetadata,
        MemoryDictionaryVectorStoreAsync<int, TMetadata>,
        DictionaryVocabularyStoreAsync<string>,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizerAsync<string, int>,
        CosineSimilarityVectorComparerAsync
        >
{
    public MemoryVectorDatabase()
        : base(
            new MemoryDictionaryVectorStoreAsync<int, TMetadata>(),
            new DictionaryVocabularyStoreAsync<string>()
            )
    { }
}
