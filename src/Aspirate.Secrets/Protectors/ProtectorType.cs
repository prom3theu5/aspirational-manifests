namespace Aspirate.Secrets.Protectors;

public class ProtectorType(string name, string value) : SmartEnum<ProtectorType, string>(name, value)
{
    public static readonly ProtectorType ConnectionString = new(nameof(ConnectionString), "ConnectionString");
    public static readonly ProtectorType PostgresPassword = new(nameof(PostgresPassword), "POSTGRES_PASSWORD");
}
