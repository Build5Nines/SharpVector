namespace Build5Nines.SharpVector.Id;

public class NumericIdGenerator<TId> : IIdGenerator<TId>
    where TId : struct
{
    public TId CurrentId { get; set; } = default(TId);

    public int GetTotalCountGenerated()
    {
        return Convert.ToInt32(CurrentId);
    }

    public TId NewId() {
        dynamic current = CurrentId;
        current++;
        CurrentId = current;
        return CurrentId;
    }
}