namespace Build5Nines.SharpVector.Id;

public interface IIdGenerator<TId>
    where TId : notnull
{
    /// <summary>
    /// Generates a new ID.
    /// </summary>
    /// <returns></returns>
    TId NewId();
}