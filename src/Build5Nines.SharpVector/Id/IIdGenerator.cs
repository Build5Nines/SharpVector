namespace Build5Nines.SharpVector.Id;

public interface IIdGenerator<TId>
    where TId : notnull
{
    /// <summary>
    /// The most recent ID generated.
    /// </summary>
    TId CurrentId { get; }

    /// <summary>
    /// Generates a new ID.
    /// </summary>
    /// <returns></returns>
    TId NewId();

    /// <summary>
    /// Returns the total count of IDs generated.
    /// </summary>
    /// <returns></returns>
    int GetTotalCountGenerated();
}