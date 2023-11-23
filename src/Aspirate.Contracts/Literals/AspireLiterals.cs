namespace Aspirate.Contracts.Literals;

[ExcludeFromCodeCoverage]
public static class AspireLiterals
{
    public const string PostgresServer = "postgres.server.v0";
    public const string PostgresDatabase = "postgres.database.v0";
    public const string Project = "project.v0";
    public const string Redis = "redis.v0";
    public const string RabbitMq = "rabbitmq.server.v0";
    public const string Final = "final";
    public static string BuildManifestCommand(string appHostProjectPath, string outputFile = "manifest.json") => $"run --project {appHostProjectPath} -- --publisher manifest --output-path {outputFile}";
}
