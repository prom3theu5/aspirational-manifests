using Aspirate.Shared.Models.AspireManifests.Components.V1;

namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceGenericConnectionStringSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public const string ConnectionStringPlaceholder = "connectionString";

    private readonly Dictionary<string, Func<string, Dictionary<string, Resource>, string>> _connectionStringMapping = new()
    {
        [AspireComponentLiterals.Redis] = (_, _) => "redis",
        [AspireComponentLiterals.PostgresDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<PostgresDatabase, PostgresServer>(resources, resourceName),
        [AspireComponentLiterals.SqlServerDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<SqlServerDatabase, SqlServer>(resources, resourceName),
        [AspireComponentLiterals.MySqlDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<MySqlDatabase, MySqlServer>(resources, resourceName),
        [AspireComponentLiterals.Container] = (resourceName, resources) => (resources[resourceName] as Container)?.ConnectionString,
        [AspireComponentLiterals.RabbitMq] = (_, _) => "amqp://guest:guest@rabbitmq-service:5672",
        [AspireComponentLiterals.MongoDbServer] = (_, _) => "mongodb://mongo-service:27017",
    };

    public bool CanSubstitute(KeyValuePair<string, string> placeholder) => placeholder.Value.Contains($".{ConnectionStringPlaceholder}", StringComparison.OrdinalIgnoreCase);

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
        var cleanPlaceholder = placeholder.Value.Trim('{', '}');
        var parts = cleanPlaceholder.Split('.');
        var resourceName = parts[0];
        var resourceType = resources[resourceName].Type;

        if (_connectionStringMapping.TryGetValue(resourceType, out var typeHandler))
        {
            resource.Env[placeholder.Key] = typeHandler(resourceName, resources);
        }
    }

    private static string SetupDatabaseConnectionString<TDatabase, TServer>(IReadOnlyDictionary<string, Resource> resources, string resourceName)
        where TDatabase : IResourceWithParent
        where TServer : Resource
    {
        if (resources[resourceName] is not TDatabase databaseResource)
        {
            throw new InvalidOperationException($"Resource {resourceName} is not a database.");
        }

        if (resources[databaseResource.Parent] is not { } parentResource)
        {
            throw new InvalidOperationException($"Parent resource {databaseResource.Parent} is not a Database Server.");
        }

        return typeof(TServer) switch
        {
            var type when type == typeof(Shared.Models.AspireManifests.Components.V1.SqlServer) => $"Server=sqlserver-service;User ID=sa;Password={(parentResource.Env["SaPassword"]).FromBase64()};TrustServerCertificate=true;",
            var type when type == typeof(MySqlServer) => $"Server=mysql-service;Port=3306;User ID=root;Password={(parentResource.Env["RootPassword"]).FromBase64()};",
            var type when type == typeof(PostgresServer) => $"host=postgres-service;database={resourceName};username=postgres;password=postgres;",
            _ => throw new InvalidOperationException($"Resource {databaseResource.Parent} is not a supported database server."),
        };
    }
}
