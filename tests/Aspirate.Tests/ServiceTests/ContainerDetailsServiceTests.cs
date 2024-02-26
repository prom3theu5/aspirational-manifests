namespace Aspirate.Tests.ServiceTests;

public class ContainerDetailsServiceTests
{
    [ModuleInitializer]
    internal static void Initialize() =>
        VerifierSettings.NameForParameter<TestContainerProperties>(name => name.Value);

    [Theory]
    [MemberData(nameof(MockContainerProperties))]
    public async Task GetContainerDetails_WhenCalled_ReturnsCorrectContainerDetails(TestContainerProperties properties)
    {
        // Arrange
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var testConsole = new TestConsole();

        var responseJson = JsonSerializer.Serialize(properties.Properties);

        projectPropertyService
            .GetProjectPropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ReturnsForAnyArgs(responseJson);

        var containerDetailsService = new ContainerDetailsService(projectPropertyService, testConsole);

        if (properties.Parameters is null)
        {
            properties = properties with { Parameters = new() };
        }

        // Act
        var containerDetails = await containerDetailsService.GetContainerDetails("test-service", new(), properties.Parameters);

        // Assert
        await Verify(containerDetails)
            .UseParameters(properties)
            .UseDirectory("VerifyResults");
    }

    public static IEnumerable<object[]> MockContainerProperties =>
        new List<object[]>
        {
            new object[]
            {
                new TestContainerProperties(
                    "FullResponse", CreateContainerProperties("test-registry", "test-repository", "test-image", "test-tag")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "FullResponseWithPrefix", CreateContainerProperties("test-registry", "test-repository", "test-image", "test-tag"), new()
                    {
                        Prefix = "test-prefix",
                    }),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoTagShouldBeLatest", CreateContainerProperties("test-registry", "test-repository", "test-image")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoImageShouldBeRepository", CreateContainerProperties("test-registry", "test-repository", null, "test-tag")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoImageShouldBeRepositoryWithPrefix", CreateContainerProperties("test-registry", "test-repository", null, "test-tag"), new ContainerParameters()
                    {
                        Prefix = "test-prefix",
                    }),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoImageOrTagShouldBeRepositoryLatest", CreateContainerProperties("test-registry", "test-repository")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoImageOrTagShouldBeRepositoryWithPrefixLatest", CreateContainerProperties("test-registry", "test-repository"), new ContainerParameters()
                    {
                        Prefix = "test-prefix",
                    }),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoRepositoryShouldBeImage", CreateContainerProperties("test-registry", null, "test-image", "test-tag")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoRepositoryOrTagShouldBeImageLatest", CreateContainerProperties("test-registry", null, "test-image")),
            },
            new object[]
            {
                new TestContainerProperties(
                    "NoRegistryOrRepositoryOrTagShouldBeImageLatest", CreateContainerProperties(null, null, "test-image")),
            },
        };

    private static MsBuildProperties<MsBuildContainerProperties> CreateContainerProperties(string? registry = null,
        string? repo = null,
        string? image = null,
        string? tag = null) =>
        new()
        {
            Properties = new()
            {
                ContainerRegistry = registry, ContainerRepository = repo, ContainerImageName = image, ContainerImageTag = tag,
            },
        };

    public record TestContainerProperties(string Value, MsBuildProperties<MsBuildContainerProperties> Properties, ContainerParameters? Parameters = null);
}
