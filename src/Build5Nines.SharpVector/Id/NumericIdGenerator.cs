namespace Build5Nines.SharpVector.Id;

public class NumericIdGenerator<TId> : IIdGenerator<TId>
    where TId : struct
{
    public NumericIdGenerator()
    { }

    public NumericIdGenerator(TId mostRecentId) : this()
    {
        this._lastId = mostRecentId;
    }

    private readonly object _lock = new object();
    private TId _lastId = default(TId);

    public TId NewId() {
        lock(_lock) {
            dynamic current = _lastId;
            current++;
            _lastId = current;
            return _lastId;
        }
    }
}