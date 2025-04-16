namespace Build5Nines.SharpVector;

public class DatabaseInfo
{
    internal const string SupportedVersion = "1.0.0";
    internal const string SupportedSchema = "Build5Nines.SharpVector";

    public DatabaseInfo()
        : this(null, null, null)
    { }
    public DatabaseInfo(string? classType)
        : this(SupportedSchema, SupportedVersion, classType)
    { }

    public DatabaseInfo(string? schema, string? version, string? classType)
    {
        Schema = schema;
        Version = version;
        ClassType = classType;
    }

    public string? Schema { get; set; }
    public string? Version { get; set; }
    public string? ClassType { get; set; }
}