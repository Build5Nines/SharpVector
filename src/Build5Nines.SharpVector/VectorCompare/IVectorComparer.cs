namespace Build5Nines.SharpVector.VectorCompare;

public interface IVectorComparer
{
    /// <summary>
    /// Calculates a comparison between two vectors
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    float Calculate(float[] vectorA, float[] vectorB);

    /// <summary>
    /// Sorts the results of a comparison
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="results"></param>
    /// <returns></returns>
    IEnumerable<VectorTextResultItem<TDocument, TMetadata>> Sort<TDocument, TMetadata>(IEnumerable<VectorTextResultItem<TDocument, TMetadata>> results);

    /// <summary>
    /// Sorts the results of a comparison
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="results"></param>
    /// <returns></returns>
    IEnumerable<VectorTextResultItem<TMetadata>> Sort<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results);

    /// <summary>
    /// Determines if the comparison is within threshold threshold
    /// </summary>
    /// <param name="threshold"></param>
    /// <param name="vectorComparisonValue"></param>
    /// <returns></returns>
    bool IsWithinThreshold(float? threshold, float vectorComparisonValue);

    /// <summary>
    /// Calculates a comparison between two vectors asynchronously
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    Task<float> CalculateAsync(float[] vectorA, float[] vectorB);

    /// <summary>
    /// Sorts the results of a comparison asynchronously
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="results"></param>
    /// <returns></returns>
    Task<IEnumerable<VectorTextResultItem<TDocument, TMetadata>>> SortAsync<TDocument, TMetadata>(IEnumerable<VectorTextResultItem<TDocument, TMetadata>> results);

    /// <summary>
    /// Sorts the results of a comparison asynchronously    
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <param name="results"></param>
    /// <returns></returns>
    Task<IEnumerable<VectorTextResultItem<TMetadata>>> SortAsync<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results);
}

