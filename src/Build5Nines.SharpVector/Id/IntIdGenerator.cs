namespace Build5Nines.SharpVector.Id;

public class IntIdGenerator : NumericIdGenerator<int>
{
    public IntIdGenerator() : base()
    { }

    public IntIdGenerator(int mostRecentId) : base(mostRecentId)
    { }
}
