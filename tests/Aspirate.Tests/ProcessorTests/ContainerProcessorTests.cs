using Aspirate.Processors.PlaceholderSubstitutionStrategies;
using Aspirate.Processors.Resources.Container;

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
        var strategies = new List<IPlaceholderSubstitutionStrategy> { new ResourceContainerConnectionStringSubstitutionStrategy() };
        var containerProcessor = new ContainerProcessor(fileSystem, console, secretProvider, containerCompositionService, containerDetailsService, manifestWriter, strategies);

        var resource = new ContainerResource
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
