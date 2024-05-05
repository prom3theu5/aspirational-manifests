using Aspirate.Shared.Enums;

namespace Aspirate.Tests.ActionsTests.Manifests;


public class GenerateKustomizeManifestsActionTests : BaseActionTests<GenerateKustomizeManifestsAction>
{
    [Fact]
    public async Task ExecuteGenerateKustomizeManifestsActionAction_ComposeOutput_ShouldThrow()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        var state = CreateAspirateState(projectPath: DefaultProjectPath, outputFormat: OutputFormat.DockerCompose.Value);
        var serviceProvider = CreateServiceProvider(state, console, new FileSystem());

        var sut = GetSystemUnderTest(serviceProvider);

        // Act
        var action = () => sut.ExecuteAsync();

        // Assert
        await action.Should().ThrowAsync<ActionCausesExitException>();
    }
}
