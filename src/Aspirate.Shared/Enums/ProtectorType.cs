namespace Aspirate.Shared.Enums;

public class ProtectorType(string name, string value) : SmartEnum<ProtectorType, string>(name, value)
{
    public static readonly ProtectorType ConnectionString = new(nameof(ConnectionString), "ConnectionString");
    public static readonly ProtectorType PostgresPassword = new(nameof(PostgresPassword), "POSTGRES_PASSWORD");
    public static readonly ProtectorType MsSqlPassword = new(nameof(MsSqlPassword), "MSSQL_SA_PASSWORD");
    public static readonly ProtectorType RabbitMQPassword = new(nameof(RabbitMQPassword), "RABBITMQ_DEFAULT_PASS");
}
