using System.Threading.Tasks;
using Aspirate.Shared.Enums;
using Xunit;

namespace Aspirate.Tests.ActionsTests.Manifests;


public class GenerateDockerComposeManifestActionTests : BaseActionTests<GenerateDockerComposeManifestAction>
{
    [Fact]
    public async Task ExecuteGenerateDockerComposeManifestAction_KustomizeOutput_ShouldThrow()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        var state = CreateAspirateState(projectPath: DefaultProjectPath, outputFormat: OutputFormat.Kustomize.Value);
        var serviceProvider = CreateServiceProvider(state, console, new FileSystem());

        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var action = () => generateAspireManifestAction.ExecuteAsync();

        // Assert
        await action.Should().ThrowAsync<ActionCausesExitException>();
    }
}
