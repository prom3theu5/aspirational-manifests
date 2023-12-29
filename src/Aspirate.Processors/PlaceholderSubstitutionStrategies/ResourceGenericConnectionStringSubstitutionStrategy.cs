namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceGenericConnectionStringSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public const string ConnectionStringPlaceholder = "connectionString";

    private readonly Dictionary<string, Func<string, Dictionary<string, Resource>, string>> _connectionStringMapping = new()
    {
        [AspireComponentLiterals.Redis] = (_, _) => "redis",
        [AspireComponentLiterals.PostgresDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<PostgresDatabaseResource>(resources, resourceName),
        [AspireComponentLiterals.SqlServerDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<SqlServerDatabaseResource>(resources, resourceName),
        [AspireComponentLiterals.MySqlDatabase] = (resourceName, resources) => SetupDatabaseConnectionString<MySqlDatabaseResource>(resources, resourceName),
        [AspireComponentLiterals.Container] = (resourceName, resources) => (resources[resourceName] as ContainerResource)?.ConnectionString,
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

    public void Reset() { }

    private static string SetupDatabaseConnectionString<TDatabase>(IReadOnlyDictionary<string, Resource> resources, string resourceName)
        where TDatabase : IResourceWithParent
    {
        if (resources[resourceName] is not TDatabase databaseResource)
        {
            throw new InvalidOperationException($"Resource {resourceName} is not a database.");
        }

        if (resources[databaseResource.Parent] is not { } parentResource)
        {
            throw new InvalidOperationException($"Parent resource {databaseResource.Parent} is not a Database Server.");
        }

        return GetConnectionString(resourceName, databaseResource, parentResource);
    }

    private static string GetConnectionString(string resourceName, IResourceWithParent databaseResource, Resource parentResource) =>
        parentResource.GetType() switch
        {
            { } t when t == typeof(SqlServerResource) => GetSqlServerConnectionString(parentResource),
            { } t when t == typeof(MySqlServerResource) => GetMySqlServerConnectionString(parentResource),
            { } t when t == typeof(PostgresServerResource) => GetPostgresServerConnectionString(resourceName, parentResource),
            { } t when t == typeof(ContainerResource) => GetContainerConnectionString(resourceName, parentResource),
            _ => throw new InvalidOperationException($"Resource {databaseResource.Parent} is not a supported database server.")
        };

    private static string GetSqlServerConnectionString(Resource parentResource) =>
        $"Server=sqlserver-service;User ID=sa;Password={(parentResource.Env["SaPassword"]).FromBase64()};TrustServerCertificate=true;";

    private static string GetMySqlServerConnectionString(Resource parentResource) =>
        $"Server=mysql-service;Port=3306;User ID=root;Password={(parentResource.Env["RootPassword"]).FromBase64()};";

    private static string GetPostgresServerConnectionString(string resourceName, Resource parentResource)
    {
        string password = string.Empty;

        if (parentResource.Env is not null)
        {
            password = HandlePostgresEnvPassword(parentResource, password);
        }

        if (string.IsNullOrEmpty(password))
        {
            password = "postgres";
        }

        return
            $"host=postgres-service;database={resourceName};username=postgres;password={password};";
    }

    private static string HandlePostgresEnvPassword(Resource parentResource, string password)
    {
        if (parentResource.Env.TryGetValue("POSTGRES_PASSWORD", out string envPassword))
        {
            password = envPassword.FromBase64();
        }

        return password;
    }

    private static string GetContainerConnectionString(string resourceName, Resource parentResource)
    {
        if (parentResource is not ContainerResource containerResource)
        {
            throw new InvalidOperationException($"Resource {resourceName} is not a container.");
        }

        return containerResource.ConnectionString;
    }
}
