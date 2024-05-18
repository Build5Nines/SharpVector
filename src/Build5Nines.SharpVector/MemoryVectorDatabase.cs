using Build5Nines.SharpVector.Data.Vocabulary;
using Build5Nines.SharpVector.Data.Id;

namespace Build5Nines.SharpVector;

public class MemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int, 
        TMetadata,
        DictionaryVocabularyStore<string>,
        IntIdGenerator
        >
{
    public MemoryVectorDatabase()
        : base(
            new DictionaryVocabularyStore<string>()
            )
    { }
}
