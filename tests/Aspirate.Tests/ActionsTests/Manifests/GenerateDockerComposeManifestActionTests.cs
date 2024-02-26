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

    [Fact]
    public async Task ExecuteGenerateDockerComposeManifestAction_ShouldGenerateDockerComposeFile()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        var state = CreateAspirateState(projectPath: DefaultProjectPath, outputFormat: OutputFormat.DockerCompose.Value);
        var serviceProvider = CreateServiceProvider(state, console, new FileSystem());

        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        console.Lines.Should().ContainSingle(x => x.Contains("Done:  Generating "));
    }

    [Fact]
    public async Task ExecuteGenerateDockerComposeManifestAction_ShouldGenerateDockerComposeOverrideFile()
    {
        // Arrange
        var composeOverrideFile = Path.Combine(AppContext.BaseDirectory,"aspirate-output", "docker-compose.override.yml");
        if (File.Exists(composeOverrideFile))
        {
            File.Delete(composeOverrideFile);
        }
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(() => CreateAspirateState(projectPath: DefaultProjectPath,
            outputFormat: OutputFormat.DockerCompose.Value,
            nonInteractive: true,
            //networkName: "nginx",
            aspireManifest: Path.Combine(DefaultProjectPath, "TestData", "preview-2-manifest.json"),
            composeOverride: true),nonInteractive: true, generatedInputs: true);

        var fileSystem = new FileSystem();
        var serviceProvider = CreateServiceProvider(state, console, fileSystem);

        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        console.Lines.Should().ContainSingle(x => x.Contains("Done:  Generating "));

        File.Exists(composeOverrideFile).Should().BeTrue();
    }
}
