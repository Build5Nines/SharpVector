namespace Build5Nines.SharpVector;

public class DatabaseInfo
{
    internal static string SupportedVersion = "1.0.0";
    internal static string SupportedSchema = "Build5Nines.SharpVector";

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