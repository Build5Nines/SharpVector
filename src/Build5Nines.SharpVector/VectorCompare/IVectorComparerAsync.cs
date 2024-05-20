namespace Build5Nines.SharpVector.VectorCompare;

public interface IVectorComparerAsync : IVectorComparer
{
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
    Task<IEnumerable<VectorTextResultItem<TMetadata>>> SortAsync<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results);
}