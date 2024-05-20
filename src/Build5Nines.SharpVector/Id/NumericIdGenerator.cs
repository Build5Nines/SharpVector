namespace Build5Nines.SharpVector.Id;

public class NumericIdGenerator<TId> : IIdGenerator<TId>
    where TId : struct
{
    private readonly object _lock = new object();
    private TId _currentId = default(TId);

    public TId CurrentId
    {
        get
        {
            lock (_lock) {
                return _currentId;
            }
        }
        set
        {
            lock (_lock) {
                _currentId = value;
            }
        }
    }

    public int GetTotalCountGenerated()
    {
        lock (_lock) {
            return Convert.ToInt32(_currentId);
        }
    }

    public TId NewId() {
        lock(_lock) {
            dynamic current = _currentId;
            current++;
            _currentId = current;
            return _currentId;
        }
    }
}