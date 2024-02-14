using Aspirate.Shared.Models.AspireManifests.Components.V0.Azure;

namespace Aspirate.Tests.ServiceTests;

[UsesVerify]
public class ManifestFileParserServiceTest
{
    [Fact]
    public void LoadAndParseAspireManifest_ShouldCallExpectedMethods_WhenCalled()
    {
        // Arrange

        var fileSystem = new MockFileSystem();
        var manifestFile = "testManifest.json";
        fileSystem.AddFile(manifestFile, new("{\"resources\": {}}"));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void LoadAndParseAspireManifest_ThrowsException_WhenManifestFileDoesNotExist()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        Action act = () => service.LoadAndParseAspireManifest("nonexistent.json");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The manifest file could not be loaded from: 'nonexistent.json'");
    }

    [Fact]
    public void LoadAndParseAspireManifest_ReturnsEmptyDictionary_WhenManifestFileIsEmpty()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "empty.json";
        fileSystem.AddFile(manifestFile, new("{}"));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void LoadAndParseAspireManifest_ReturnsUnsupportedResource_WhenResourceTypeIsMissing()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "missingType.json";
        fileSystem.AddFile(manifestFile, new("{\"resources\": {\"resource1\": {}}}"));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().HaveCount(1);
        result["resource1"].Should().BeOfType<UnsupportedResource>();
    }

    [Fact]
    public void LoadAndParseAspireManifest_ReturnsUnsupportedResource_WhenResourceTypeIsUnsupported()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "unsupportedType.json";
        fileSystem.AddFile(manifestFile, new("{\"resources\": {\"resource1\": {\"type\": \"unsupported\"}}}"));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().HaveCount(1);
        result["resource1"].Should().BeOfType<UnsupportedResource>();
    }

    [Fact]
    public void LoadAndParseAspireManifest_ReturnsResource_WhenResourceTypeIsSupported()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "supportedType.json";
        fileSystem.AddFile(
            manifestFile, new("{\"resources\": {\"resource1\": {\"type\": \"postgres.database.v0\"}}}"));

        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().HaveCount(1);
        result["resource1"].Should().NotBeOfType<UnsupportedResource>();
        result["resource1"].Should().BeOfType<PostgresDatabaseResource>();
    }

    [Fact]
    public async Task LoadAndParseAspireManifest_ParsesManifestFileCorrectly()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "manifest.json";
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(File.ReadAllText(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var state = serviceProvider.GetRequiredService<AspirateState>();
        state.LoadedAspireManifestResources = service.LoadAndParseAspireManifest(manifestFile);

        var postgresContainer = state.LoadedAspireManifestResources["postgrescontainer"] as ContainerResource;
        postgresContainer.Inputs["password"].Value = "secret_password"; // inputs captured from user input

        var postLoadAction = new SubstituteValuesAspireManifestAction(serviceProvider);
        await postLoadAction.ExecuteAsync();
        var result = state.LoadedAspireManifestResources;

        // Assert
        result.Should().HaveCount(15);
        result["postgres"].Should().BeOfType<PostgresServerResource>();

        result["azurekeyvault"].Should().BeOfType<AzureKeyVaultResource>();
        result["azurestorage"].Should().BeOfType<AzureStorageResource>();
        result["azurestorageblob"].Should().BeOfType<AzureStorageBlobResource>();

        result["sqlserver"].Should().BeOfType<SqlServerResource>();
        result["sqlserver"].Env["SaPassword"].Should().NotBeNull().And.NotBeEmpty();
        result["sqldb"].Should().BeOfType<SqlServerDatabaseResource>();

        result["mysqlserver"].Should().BeOfType<MySqlServerResource>();
        result["mysqlserver"].Env["RootPassword"].Should().NotBeNull().And.NotBeEmpty();
        result["mysqldb"].Should().BeOfType<MySqlDatabaseResource>();

        result["mongodbserver"].Should().BeOfType<MongoDbServerResource>();
        result["mongodbdb"].Should().BeOfType<MongoDbDatabaseResource>();

        result["catalogdb"].Should().BeOfType<PostgresDatabaseResource>();

        result["basketcache"].Should().BeOfType<RedisResource>();

        result["catalogservice"].Should().BeOfType<ProjectResource>();
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"].Should().Be("true");
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"].Should().Be("true");

        result["anotherservice"].Should().BeOfType<ProjectResource>();
        result["anotherservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES");
        result["anotherservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"].Should().Be("true");
        result["anotherservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES");
        result["anotherservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"].Should().Be("true");
        result["anotherservice"].Env["services__catalogservice"].Should().Be("http://catalogservice:8080");
        result["anotherservice"].Env["ConnectionStrings__basketcache"].Should().Be("redis");
        result["anotherservice"].Env["ConnectionStrings__postgrescontainer"].Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password;");

        postgresContainer = result["postgrescontainer"] as ContainerResource;
        postgresContainer.ConnectionString.Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password;");
    }

     [Fact]
    public async Task LoadAndParseAspirePreviewTwoManifest_ParsesManifestFileCorrectly()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "preview-2-manifest.json";
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(File.ReadAllText(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var state = serviceProvider.GetRequiredService<AspirateState>();
        state.LoadedAspireManifestResources = service.LoadAndParseAspireManifest(manifestFile);

        var postgresContainer = state.LoadedAspireManifestResources["catalog"] as ContainerResource;
        postgresContainer.Inputs["password"].Value = "secret_password"; // inputs captured from user input

        var substituteValuesAspireManifestAction = new SubstituteValuesAspireManifestAction(serviceProvider);
        await substituteValuesAspireManifestAction.ExecuteAsync();

        var applyDaprAnnotationsAction = new ApplyDaprAnnotationsAction(serviceProvider, new TestConsole());
        await applyDaprAnnotationsAction.ExecuteAsync();

        var result = state.LoadedAspireManifestResources;

        // Assert
        result.Should().HaveCount(10);
        result["catalog"].Should().BeOfType<ContainerResource>();
        result["catalogdb"].Should().BeOfType<PostgresDatabaseResource>();
        result["basketcache"].Should().BeOfType<ContainerResource>();
        result["catalogservice-dapr"].Should().BeOfType<DaprResource>();

        result["catalogservice"].Should().BeOfType<ProjectResource>();
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"].Should().Be("true");
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"].Should().Be("true");
        result["catalogservice"].Env["ConnectionStrings__catalogdb"].Should().Be("Host=catalog;Port=5432;Username=postgres;Password=secret_password;");
        result["catalogservice"].Annotations.Should().NotBeEmpty();
        result["catalogservice"].Annotations.Should().ContainKey("dapr.io/enabled");
        result["catalogservice"].Annotations["dapr.io/enabled"].Should().Be("true");
        result["catalogservice"].Annotations.Should().ContainKey("dapr.io/app-id");
        result["catalogservice"].Annotations["dapr.io/app-id"].Should().Be("catalogservice");

        result["basketservice"].Env["ConnectionStrings__basketcache"].Should().Be("basketcache:6379");

        postgresContainer = result["catalog"] as ContainerResource;
        postgresContainer.ConnectionString.Should().Be("Host=catalog;Port=5432;Username=postgres;Password=secret_password;");
    }

    private static IServiceProvider CreateServiceProvider(IFileSystem? fileSystem = null, IAnsiConsole? console = null)
    {
        console ??= new TestConsole();
        fileSystem ??= new FileSystem();
        var services = new ServiceCollection();
        services.RegisterAspirateEssential();
        services.RemoveAll<IAnsiConsole>();
        services.RemoveAll<IFileSystem>();
        services.AddSingleton<IAnsiConsole>(console);
        services.AddSingleton<IFileSystem>(fileSystem);
        services.AddSingleton<ISecretProvider, Base64SecretProvider>();

        return services.BuildServiceProvider();
    }
}
