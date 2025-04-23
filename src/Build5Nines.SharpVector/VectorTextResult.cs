
namespace Build5Nines.SharpVector;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a result of a vector text search.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public interface IVectorTextResult<TId, TDocument, TMetadata>
{
    /// <summary>
    /// The list of Texts found in the search results.
    /// </summary>
    IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> Texts { get; }

    /// <summary>
    /// Returns true if the search returned no results.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// The total count of Texts found in the search results.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// The current page index of the search results.
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// The total number of pages of search results.
    /// </summary>
    public int TotalPages { get; }
}

/// <summary>
/// Represents a result of a vector text search.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public interface IVectorTextResult<TMetadata>
 : IVectorTextResult<int, string, TMetadata>
 { }

/// <summary>
/// Represents a result of a vector text search.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public class VectorTextResult<TId, TDocument, TMetadata>
    : IVectorTextResult<TId, TDocument, TMetadata>
{
    public VectorTextResult(int totalCount, int pageIndex, int totalPages, IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> texts)
    {
        Texts = texts;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        TotalPages = totalPages;
    }

    /// <summary>
    /// Returns true if the search returned no results.
    /// </summary>
    public IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> Texts { get; private set; }

    /// <summary>
    /// Returns true if the search returned no results.
    /// </summary>
    public bool IsEmpty { get => Texts == null || !Texts.Any(); }

    /// <summary>
    /// The total count of Texts found in the search results.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// The current page index of the search results.
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// The total number of pages of search results.
    /// </summary>
    public int TotalPages { get; private set; }
}

/// <summary>
/// Represents a result of a vector text search.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
public class VectorTextResult<TMetadata>
    : VectorTextResult<int, string, TMetadata>, IVectorTextResult<TMetadata>
{
    public VectorTextResult(int totalCount, int pageIndex, int totalPages, IEnumerable<IVectorTextResultItem<int, string, TMetadata>> texts)
        : base(totalCount, pageIndex, totalPages, texts)
    { }
}