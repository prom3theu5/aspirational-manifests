namespace Aspirate.Tests.ServiceTests;

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
            manifestFile, new("{\"resources\": {\"resource1\": {\"type\": \"container.v0\", \"image\": \"some-image\"}}}"));

        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();

        // Act
        var result = service.LoadAndParseAspireManifest(manifestFile);

        // Assert
        result.Should().HaveCount(1);
        result["resource1"].Should().NotBeOfType<UnsupportedResource>();
        result["resource1"].Should().BeOfType<ContainerResource>();
    }

    [Theory]
    [InlineData("pg-endtoend.json", 22)]
    [InlineData("sqlserver-endtoend.json", 4)]
    [InlineData("starter-with-redis.json", 3)]
    [InlineData("project-no-binding.json", 1)]
    [InlineData("connectionstring-resource-expression.json", 5)]
    [InlineData("with-unsupported-resource.json", 6)]
    public async Task EndToEnd_ParsesSuccessfully(string manifestFile, int expectedCount)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(await File.ReadAllTextAsync(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);
        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();
        var inputPopulator = serviceProvider.GetRequiredKeyedService<IAction>(nameof(PopulateInputsAction));
        var valueSubstitutor = serviceProvider.GetRequiredKeyedService<IAction>(nameof(SubstituteValuesAspireManifestAction));

        await PerformEndToEndTests(manifestFile, expectedCount, serviceProvider, service, inputPopulator, valueSubstitutor);
    }

    [Fact]
    public async Task EndToEndWithManualEntry_ParsesSuccessfully()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "starter-with-db.json";
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(await File.ReadAllTextAsync(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);

        var console = serviceProvider.GetRequiredService<IAnsiConsole>() as TestConsole;
        console.Profile.Capabilities.Interactive = true;
        EnterPasswordInput(console, "secret_password");

        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();
        var inputPopulator = serviceProvider.GetRequiredKeyedService<IAction>(nameof(PopulateInputsAction));
        var valueSubstitutor = serviceProvider.GetRequiredKeyedService<IAction>(nameof(SubstituteValuesAspireManifestAction));

        await PerformEndToEndTests(manifestFile, 8, serviceProvider, service, inputPopulator, valueSubstitutor);
    }

    [Fact]
    public async Task EndToEndShop_ParsesSuccessfully()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "shop.json";
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(await File.ReadAllTextAsync(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);

        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();
        var inputPopulator = serviceProvider.GetRequiredKeyedService<IAction>(nameof(PopulateInputsAction));
        var valueSubstitutor = serviceProvider.GetRequiredKeyedService<IAction>(nameof(SubstituteValuesAspireManifestAction));

        var results = await PerformEndToEndTests(manifestFile, 12, serviceProvider, service, inputPopulator, valueSubstitutor);

        var shopResource = results["basketcache"] as ContainerResource;
        shopResource.Volumes.Should().HaveCount(1);
        shopResource.Volumes[0].Name.Should().Be("basketcache-data");
    }

    [Fact]
    public async Task EndToEndNodeJs_ParsesSuccessfully()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var manifestFile = "nodejs.json";
        var testData = Path.Combine(AppContext.BaseDirectory, "TestData", manifestFile);
        fileSystem.AddFile(manifestFile, new(await File.ReadAllTextAsync(testData)));
        var serviceProvider = CreateServiceProvider(fileSystem);

        var service = serviceProvider.GetRequiredService<IManifestFileParserService>();
        var inputPopulator = serviceProvider.GetRequiredKeyedService<IAction>(nameof(PopulateInputsAction));
        var valueSubstitutor = serviceProvider.GetRequiredKeyedService<IAction>(nameof(SubstituteValuesAspireManifestAction));
        var cachePopulator = serviceProvider.GetRequiredKeyedService<IAction>(nameof(BuildAndPushContainersFromDockerfilesAction));
        var state = serviceProvider.GetRequiredService<AspirateState>();

        // Act
        state.SkipBuild = true;
        await PerformEndToEndTests(manifestFile, 1, serviceProvider, service, inputPopulator, valueSubstitutor);
        state.AspireComponentsToProcess = state.LoadedAspireManifestResources.Select(x=>x.Key).ToList();
        await cachePopulator.ExecuteAsync();
    }

    private static async Task<Dictionary<string, Resource>> PerformEndToEndTests(string manifestFile, int expectedCount, IServiceProvider serviceProvider, IManifestFileParserService service, IAction inputPopulator, IAction valueSubstitutor)
    {
        // Act
        var state = serviceProvider.GetRequiredService<AspirateState>();
        state.LoadedAspireManifestResources = service.LoadAndParseAspireManifest(manifestFile);
        await inputPopulator.ExecuteAsync();
        await valueSubstitutor.ExecuteAsync();
        var result = state.LoadedAspireManifestResources;

        // Assert
        result.Should().HaveCount(expectedCount);

        foreach (var container in result.Where(x => x.Value is ContainerResource))
        {
            var containerResource = container.Value as ContainerResource;
            containerResource.ConnectionString.Should().NotBeNullOrEmpty();
            containerResource.ConnectionString.Should().NotContain("{");
            containerResource.ConnectionString.Should().NotContain("}");

            foreach (var envVar in containerResource.Env)
            {
                envVar.Value.Should().NotContain("{");
                envVar.Value.Should().NotContain("}");
            }
        }

        foreach (var project in result.Where(x => x.Value is ProjectResource))
        {
            var containerResource = project.Value as ProjectResource;

            if (containerResource.Env is null)
            {
                continue;
            }

            foreach (var envVar in containerResource.Env)
            {
                envVar.Value.Should().NotContain("{");
                envVar.Value.Should().NotContain("}");
            }
        }

        return result;
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

    private static void EnterPasswordInput(TestConsole console, string password)
    {
        // first entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);

        // confirmation entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);
    }
}
