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
        result["resource1"].Should().BeOfType<PostgresDatabase>();
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

        var postgresContainer = state.LoadedAspireManifestResources["postgrescontainer"] as Container;
        postgresContainer.Inputs["password"].Value = "secret_password"; // inputs captured from user input

        var postLoadAction = new SubstituteValuesAspireManifestAction(serviceProvider);
        await postLoadAction.ExecuteAsync();
        var result = state.LoadedAspireManifestResources;

        // Assert
        result.Should().HaveCount(6);
        result["postgres"].Should().BeOfType<PostgresServer>();
        result["catalogdb"].Should().BeOfType<PostgresDatabase>();
        result["basketcache"].Should().BeOfType<Redis>();
        result["catalogservice"].Should().BeOfType<Project>();
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"].Should().Be("true");
        result["catalogservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES");
        result["catalogservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"].Should().Be("true");
        result["anotherservice"].Should().BeOfType<Project>();
        result["anotherservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES");
        result["anotherservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"].Should().Be("true");
        result["anotherservice"].Env.Should().ContainKey("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES");
        result["anotherservice"].Env["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"].Should().Be("true");
        result["anotherservice"].Env["services__catalogservice"].Should().Be("http://catalogservice:8080");
        result["anotherservice"].Env["ConnectionStrings__basketcache"].Should().Be("redis");
        result["anotherservice"].Env["ConnectionStrings__postgrescontainer"].Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password;");

        postgresContainer = result["postgrescontainer"] as Container;
        postgresContainer.ConnectionString.Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password;");
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

        return services.BuildServiceProvider();
    }
}
