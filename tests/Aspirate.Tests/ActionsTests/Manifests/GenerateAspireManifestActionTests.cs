using Aspirate.Shared.Outputs;

namespace Aspirate.Tests.ActionsTests.Manifests;

public class GenerateAspireManifestActionTests : BaseActionTests<GenerateAspireManifestAction>
{
    [Fact]
    public async Task ExecuteGenerateAspireManifestAction_ManifestProvided_Success()
    {
        // Arrange
        var aspireManifest = Path.Combine(AppContext.BaseDirectory, "TestData", "manifest.json");
        var state = CreateAspirateState(aspireManifest: aspireManifest);
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteGenerateAspireManifestAction_ManifestNotProvided_Success()
    {
        // Arrange
        var state = CreateAspirateState(projectPath: DefaultProjectPath);
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var generateAspireManifestAction = GetSystemUnderTest(serviceProvider);

        var mockExecutorService = serviceProvider.GetRequiredService<IShellExecutionService>();
        mockExecutorService.ClearSubstitute();
        mockExecutorService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(new ShellCommandResult(true, string.Empty, string.Empty, 0));

        // Act
        var result = await generateAspireManifestAction.ExecuteAsync();

        // Assert
        await mockExecutorService.Received(1).ExecuteCommand(
            Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));

        result.Should().BeTrue();
    }
}
