namespace Build5Nines.SharpVector.Similarity;

public interface IVectorSimilarityCalculatorAsync
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
    Task<float> CalculateVectorSimilarityAsync(float[] vectorA, float[] vectorB);
}