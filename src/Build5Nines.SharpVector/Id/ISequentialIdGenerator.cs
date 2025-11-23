namespace Build5Nines.SharpVector.Id;

/// <summary>
/// Interface for ID generators that support setting the most recent generated ID (sequential/numeric style).
/// </summary>
/// <typeparam name="TId">The ID type.</typeparam>
public interface ISequentialIdGenerator<TId> : IIdGenerator<TId>
    where TId : notnull
{
    /// <summary>
    /// Sets the most recent ID value so the next generated ID will continue the sequence.
    /// </summary>
    /// <param name="mostRecentId">The most recently used/generated ID.</param>
    void SetMostRecent(TId mostRecentId);
}
