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
        var containerProcessor = new ContainerProcessor(fileSystem, console, secretProvider,containerCompositionService, containerDetailsService);

        var resource = new Container
        {
            Type = AspireComponentLiterals.Container,
            ConnectionString = "Host={postgrescontainer.bindings.tcp.host};Port={postgrescontainer.bindings.tcp.port};Username=postgres;Password={postgrescontainer.inputs.password}",
            Inputs = new()
            {
                {
                    "password", new()
                    {
                        Value = "secret_password",
                    }
                },
            },
            Bindings = new()
            {
                {
                    "tcp", new()
                    {
                        Scheme = "tcp",
                        Protocol = "tcp",
                        Transport = "tcp",
                        ContainerPort = 5432,
                    }
                },
            },
            Image = "postgres:latest",
        };

        var resources = new Dictionary<string, Resource> { { "postgrescontainer", resource } };

        // Act
        containerProcessor.ReplacePlaceholders(resource, resources);

        // Assert
        resource.ConnectionString.Should().Be("Host=postgrescontainer;Port=5432;Username=postgres;Password=secret_password");
    }
}
