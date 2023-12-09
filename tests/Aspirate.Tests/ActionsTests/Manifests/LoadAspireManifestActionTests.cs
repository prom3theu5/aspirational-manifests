namespace Aspirate.Tests.ActionsTests.Manifests;

public class LoadAspireManifestActionTests : BaseActionTests<LoadAspireManifestAction>
{
    [Fact]
    public async Task ExecuteLoadAspireManifestAction_InteractiveMode_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushKey(ConsoleKey.Spacebar);
        console.Input.PushKey(ConsoleKey.Enter);

        var aspireManifest = Path.Combine(AppContext.BaseDirectory, "TestData", "manifest.json");
        var state = CreateAspirateState(aspireManifest: aspireManifest);

        var serviceProvider = CreateServiceProvider(state, console, new FileSystem());
        var loadAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await loadAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        state.SelectedProjectComponents.Should().HaveCount(2);
        state.LoadedAspireManifestResources.Keys.Count.Should().Be(10);
    }

    [Fact]
    public async Task ExecuteLoadAspireManifestAction_NonInteractiveMode_Success()
    {
        // Arrange
        var aspireManifest = Path.Combine(AppContext.BaseDirectory, "TestData", "manifest.json");
        var state = CreateAspirateState(nonInteractive: true, aspireManifest: aspireManifest);
        var serviceProvider = CreateServiceProvider(state, fileSystem: new FileSystem());
        var loadAspireManifestAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await loadAspireManifestAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        state.SelectedProjectComponents.Should().HaveCount(2);
        state.LoadedAspireManifestResources.Keys.Count.Should().Be(10);
    }
}
