namespace Build5Nines.SharpVector.VectorCompare;

public class EuclideanDistanceVectorComparerAsync : IVectorComparer, IVectorComparerAsync
{
    /// <summary>
    /// Calculates the Euclidean distance between two vectors.
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<float> CalculateAsync(float[] vectorA, float[] vectorB)
    {
        return await Task.Run(() => Calculate(vectorA, vectorB));
    }

    /// <summary>
    /// Calculates the Euclidean distance between two vectors.
    /// </summary>
    /// <param name="vectorA"></param>
    /// <param name="vectorB"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float Calculate(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must be of the same length.");
        }

        float sumOfSquares = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            float difference = vectorA[i] - vectorB[i];
            sumOfSquares += difference * difference;
        }

        return (float)Math.Sqrt(sumOfSquares);
    }

    public IEnumerable<VectorTextResultItem<TMetadata>> Sort<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results)
    {
        return results.OrderBy(s => s.Similarity);
    }

    public async Task<IEnumerable<VectorTextResultItem<TMetadata>>> SortAsync<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results)
    {
        return await Task.Run(() => Sort(results));
    }

    public bool IsWithinThreshold(float? threshold, float vectorComparisonValue)
    {
        if (threshold == null)
        {
            return true;
        }
        var thresholdToCompare = threshold ?? (float)0.0f;
        var thresholdIsEqual = Math.Abs(vectorComparisonValue - thresholdToCompare) < 1e-6f; // epsilon;
        return thresholdIsEqual || vectorComparisonValue <= thresholdToCompare;
    }
}