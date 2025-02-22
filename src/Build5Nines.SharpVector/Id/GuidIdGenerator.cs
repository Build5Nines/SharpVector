namespace Build5Nines.SharpVector.Id;

public class GuidIdGenerator : IIdGenerator<System.Guid>
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}