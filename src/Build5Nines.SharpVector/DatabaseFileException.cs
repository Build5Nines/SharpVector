namespace Build5Nines.SharpVector;

public class DatabaseFileException : Exception
{
    public DatabaseFileException()
    {
    }

    public DatabaseFileException(string message)
        : base(message)
    {
    }

    public DatabaseFileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DatabaseFileInfoException : DatabaseFileException
{
    public DatabaseFileInfoException()
    {
    }

    public DatabaseFileInfoException(string message)
        : base(message)
    {
    }

    public DatabaseFileInfoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DatabaseFileSchemaException : DatabaseFileException
{
    public DatabaseFileSchemaException()
    {
    }

    public DatabaseFileSchemaException(string message)
        : base(message)
    {
    }

    public DatabaseFileSchemaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DatabaseFileVersionException : DatabaseFileException
{
    public DatabaseFileVersionException()
    {
    }

    public DatabaseFileVersionException(string message)
        : base(message)
    {
    }

    public DatabaseFileVersionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DatabaseFileClassTypeException : DatabaseFileException
{
    public DatabaseFileClassTypeException()
    {
    }

    public DatabaseFileClassTypeException(string message)
        : base(message)
    {
    }

    public DatabaseFileClassTypeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DatabaseFileMissingEntryException : DatabaseFileException
{
    public DatabaseFileMissingEntryException(string message, string missingEntry)
        : base(message)
    {
        MissingEntry = missingEntry;
    }

    public string MissingEntry { get; private set; }
}