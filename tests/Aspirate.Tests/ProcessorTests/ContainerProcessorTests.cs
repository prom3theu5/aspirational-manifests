namespace Aspirate.Tests.ProcessorTests;

public class ContainerProcessorTests
{
    [Fact]
    public void ReplacePlaceholders_ReplacesPlaceholdersInConnectionString()
    {
        // Arrange
        var fileSystem = new FileSystem();
        var console = new TestConsole();
        var containerCompositionService = Substitute.For<IContainerCompositionService>();
        var containerDetailsService = Substitute.For<IContainerDetailsService>();
        var secretProvider = Substitute.For<ISecretProvider>();
        var manifestWriter = Substitute.For<IManifestWriter>();
        var strategies =
            new List<IPlaceholderSubstitutionStrategy> { new ResourceConnectionStringSubstitutionStrategy() };
        var containerProcessor = new ContainerProcessor(fileSystem, console, secretProvider, containerCompositionService,
            containerDetailsService, manifestWriter, strategies);

        var resource = new ContainerResource
        {
            Type = AspireComponentLiterals.Container,
            ConnectionString =
                "Host={postgrescontainer.bindings.tcp.host};Port={postgrescontainer.bindings.tcp.port};Username=postgres;Password={postgres-password.value}",
            Bindings = new()
            {
                {
                    "tcp", new()
                    {
                        Scheme = "tcp", Protocol = "tcp", Transport = "tcp", TargetPort = 5432,
                    }
                },
            },
            Image = "postgres:latest",
        };

        var inputResource = new ParameterResource { Name = "postgres-password", Value = "secret_password", Inputs = new() { { "value", new() }, }, };

        var resources = new Dictionary<string, Resource>
        {
            { "postgrescontainer", resource },
            { "postgres-password", inputResource },
        };

        // Act
        containerProcessor.ReplacePlaceholders(resource, resources);

        // Assert
        resource.ConnectionString.Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password");
    }
}
