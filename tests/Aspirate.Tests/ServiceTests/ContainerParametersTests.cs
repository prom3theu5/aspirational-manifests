namespace Aspirate.Tests.ServiceTests;

[UsesVerify]
public class ContainerParametersTests
{
    [ModuleInitializer]
    internal static void Initialize() =>
        VerifierSettings.NameForParameter<TestContainerParameters>(name => name.Value);

    [Theory]
    [MemberData(nameof(MockContainerParameters))]
    public async Task ContainerParametersFullyPopulated_ShouldPopulateImageCorrectly(TestContainerParameters testParameters)
    {
        // Arrange
        var containerParameters = testParameters.Parameters;


        // Act
        var fullImageName = containerParameters.ToImageName();


        // Assert
        await Verify(fullImageName)
            .UseParameters(testParameters)
            .UseDirectory("VerifyResults");
    }

     public static IEnumerable<object[]> MockContainerParameters =>
        new List<object[]>
        {
            new object[]
            {
                new TestContainerParameters(
                    "FullParameters", CreateContainerParameters("test-registry", "test-repository", "test-image", "test-tag")),
            },
            new object[]
            {
                new TestContainerParameters(
                    "RegistryAndPrefixAndImage", CreateContainerParameters("test-registry", "test-repository", "test-image", null)),
            },
            new object[]
            {
                new TestContainerParameters(
                    "ImageAndTag", CreateContainerParameters(null, null, "test-image", "test-tag")),
            },
        };

     private static ContainerParameters CreateContainerParameters(string? testRegistry, string? testRepositoryPrefix, string? testImage, string? testTag) =>
         new()
         {
             Registry = testRegistry,
             Prefix = testRepositoryPrefix,
             ImageName = testImage,
             Tag = testTag,
         };

     public record TestContainerParameters(string Value, ContainerParameters Parameters);
}
