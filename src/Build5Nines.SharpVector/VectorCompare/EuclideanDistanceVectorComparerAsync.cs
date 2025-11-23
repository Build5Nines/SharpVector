using System.Runtime.InteropServices;

namespace Build5Nines.SharpVector.VectorCompare;

public class EuclideanDistanceVectorComparer : IVectorComparer
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

        float sumOfSquares = 0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            float difference = vectorA[i] - vectorB[i];
            sumOfSquares += difference * difference;
        }

        return (float)Math.Sqrt(sumOfSquares);
    }

    public IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> Sort<TId, TDocument, TMetadata>(IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> results)
    {
        return results.OrderBy(s => s.Similarity);
    }

    public async Task<IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>>> SortAsync<TId, TDocument, TMetadata>(IEnumerable<IVectorTextResultItem<TId, TDocument, TMetadata>> results)
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
        return thresholdIsEqual || vectorComparisonValue < thresholdToCompare;
    }
}