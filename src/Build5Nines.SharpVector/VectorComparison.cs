namespace Build5Nines.SharpVector;

/// <summary>
/// The kind of similarity/distance metric a <see cref="VectorCompare.IVectorComparer"/>
/// represents. Used by the encoding subsystem to dispatch to the correct
/// fast-path implementation for an encoded vector.
/// </summary>
public enum VectorComparison
{
    /// <summary>Cosine similarity: higher is more similar, range [-1, 1].</summary>
    CosineSimilarity,

    /// <summary>Euclidean distance: lower is more similar, range [0, infinity).</summary>
    EuclideanDistance
}
