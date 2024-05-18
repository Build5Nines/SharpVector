using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.Similarity;

namespace Build5Nines.SharpVector;

/// <summary>
/// A simple in-memory database for storing and querying vectorized text items.
/// This database uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.
/// </summary>
/// <typeparam name="TId">Defines the data</typeparam>
/// <typeparam name="TMetadata">Defines the data type for the Metadata stored with the Text.</typeparam>
public class MemoryVectorDatabase<TId, TMetadata>
     : MemoryVectorDatabaseBase<
        int, 
        TMetadata,
        DictionaryVocabularyStore<string>,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizer<string, int>,
        CosineVectorSimilarityCalculator
        >
{
    public MemoryVectorDatabase()
        : base(
            new DictionaryVocabularyStore<string>()
            )
    { }
}
