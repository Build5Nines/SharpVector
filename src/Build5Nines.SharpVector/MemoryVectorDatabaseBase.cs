using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.Vocabulary;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.VectorStore;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Build5Nines.SharpVector.Embeddings;
using System.Runtime.ExceptionServices;
using System.Collections;
using System.Linq;

namespace Build5Nines.SharpVector;

/// <summary>
/// Base class for a memory vector database.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVectorStore"></typeparam>
/// <typeparam name="TVocabularyStore"></typeparam>
/// <typeparam name="TVocabularyKey"></typeparam>
/// <typeparam name="TVocabularyValue"></typeparam>
/// <typeparam name="TIdGenerator"></typeparam>
/// <typeparam name="TTextPreprocessor"></typeparam>
/// <typeparam name="TVectorizer"></typeparam>
/// <typeparam name="TVectorComparer"></typeparam>
public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TVocabularyStore, TVocabularyKey, TVocabularyValue, TIdGenerator, TTextPreprocessor, TVectorizer, TVectorComparer>
    : VectorDatabaseBase<TId, TMetadata, TVectorStore, TVocabularyStore, TVocabularyKey, TVocabularyValue, TIdGenerator, TTextPreprocessor, TVectorizer, TVectorComparer>
    where TId : notnull
    where TVocabularyKey : notnull
    where TVocabularyValue: notnull
    where TVectorStore : IVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
    where TVocabularyStore : IVocabularyStore<TVocabularyKey, TVocabularyValue>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TTextPreprocessor : ITextPreprocessor<TVocabularyKey>, new()
    where TVectorizer : IVectorizer<TVocabularyKey, TVocabularyValue>, new()
    where TVectorComparer : IVectorComparer, new()
{
    protected MemoryVectorDatabaseBase(TVectorStore vectorStore)
        : base(vectorStore)
    { }
}

/// <summary>
/// Base class for a memory vector database.
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
/// <typeparam name="TVectorStore"></typeparam>
/// <typeparam name="TIdGenerator"></typeparam>
/// <typeparam name="TVectorComparer"></typeparam>
public abstract class MemoryVectorDatabaseBase<TId, TMetadata, TVectorStore, TIdGenerator, TVectorComparer>
    : VectorDatabaseBase<TId, TMetadata, TVectorStore, TIdGenerator, TVectorComparer>, IMemoryVectorDatabase<TId, TMetadata>
    where TId : notnull
    where TVectorStore : IVectorStore<TId, TMetadata, string>
    where TIdGenerator : IIdGenerator<TId>, new()
    where TVectorComparer : IVectorComparer, new()
{
    public MemoryVectorDatabaseBase(IEmbeddingsGenerator embeddingsGenerator, TVectorStore vectorStore)
        : base(embeddingsGenerator, vectorStore)
    { }
}
