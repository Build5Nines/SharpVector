using System.Reflection;

namespace Build5Nines.SharpVector.VectorCompare;

public class CosineSimilarityVectorComparer : IVectorComparer
{
    /// <summary>
    /// Calculates the cosine similarity between two vectors.
    /// 
    /// Cosine Similarity is a metric used to measure how similar two vectors are. It calculates the cosine of the angle between two vectors projected in a multi-dimensional space. The result of the cosine similarity ranges from -1 to 1.
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
    /// Calculates the cosine similarity between two vectors.
    /// 
    /// Cosine Similarity is a metric used to measure how similar two vectors are. It calculates the cosine of the angle between two vectors projected in a multi-dimensional space. The result of the cosine similarity ranges from -1 to 1.
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

        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }

    public IEnumerable<VectorTextResultItem<TMetadata>> Sort<TMetadata>(IEnumerable<VectorTextResultItem<TMetadata>> results)
    {
        return results.OrderByDescending(s => s.VectorComparison);
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
        return thresholdIsEqual || vectorComparisonValue > thresholdToCompare;
    }
}