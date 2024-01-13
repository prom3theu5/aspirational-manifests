using Aspirate.Commands.Actions.Configuration;

namespace Aspirate.Tests.ActionsTests.Configuration;

[UsesVerify]
public class InitializeConfigurationActionTests : BaseActionTests<InitializeConfigurationAction>
{
    [Fact]
    public void InitializeConfigurationAction_ValidateStateNonInteractiveNullProjectPath_ThrowsActionCausesExitException()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, projectPath: null);
        var serviceProvider = CreateServiceProvider(state);
        var initializeConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => initializeConfigurationAction.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void InitializeConfigurationAction_ValidateStateNonInteractiveNullContainerRegistry_ThrowsActionCausesExitException()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, containerRegistry: null);
        var serviceProvider = CreateServiceProvider(state);
        var loadConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => loadConfigurationAction.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void InitializeConfigurationAction_ValidateStateNonInteractiveNullContainerImageTag_ThrowsActionCausesExitException()
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, containerImageTag: null);
        var serviceProvider = CreateServiceProvider(state);
        var initializeConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => initializeConfigurationAction.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public void InitializeConfigurationAction_ValidateStateNonInteractiveNullTemplatePath_DoesNotThrow(string builder)
    {
        // Arrange
        var state = CreateAspirateState(nonInteractive: true, templatePath: null, containerBuilder: builder);
        var serviceProvider = CreateServiceProvider(state);
        var initializeConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => initializeConfigurationAction.ValidateNonInteractiveState();

        // Assert
        act.Should().NotThrow<ActionCausesExitException>();
    }

    [Fact]
    public async Task ExecuteInitializeConfigurationAction_InteractiveMode_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("y");
        console.Input.PushKey(ConsoleKey.Enter);
        console.Input.PushTextWithEnter("y");
        console.Input.PushTextWithEnter("localhost:5001");
        console.Input.PushTextWithEnter("y");
        console.Input.PushTextWithEnter("prefix");
        console.Input.PushTextWithEnter("y");
        console.Input.PushTextWithEnter("tests");
        console.Input.PushTextWithEnter("n");

        var fileSystem = new MockFileSystem();
        fileSystem.Directory.CreateDirectory(DefaultProjectPath);
        var state = CreateAspirateState(nonInteractive: false, containerRegistry: null, containerImageTag: null, templatePath: null);
        var serviceProvider = CreateServiceProvider(state, console, fileSystem);

        var initializeConfigurationAction = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await initializeConfigurationAction.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        var aspirateSettingsJson = await fileSystem.File.ReadAllTextAsync(fileSystem.Path.Combine(DefaultProjectPath, AspirateSettings.FileName));
        var aspirateSettings = JsonSerializer.Deserialize<AspirateSettings>(aspirateSettingsJson);
        await Verify(aspirateSettings)
            .UseDirectory("VerifyResults");
    }
}
