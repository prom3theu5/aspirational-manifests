namespace Aspirate.Tests.ActionsTests.Secrets;

public class PopulateInputsActionTests : BaseActionTests<PopulateInputsAction>
{
    [Fact]
    public async Task ExecuteAsync_InInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        EnterPasswordInput(console, "secret_password"); // postgrescontainer
        EnterPasswordInput(console, "other_secret_password"); // postgresContainer2
        var state = CreateAspirateStateWithInputs();
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        var postgres = state.LoadedAspireManifestResources["postgrescontainer"] as IResourceWithInput;
        var postgres2 = state.LoadedAspireManifestResources["postgrescontainer2"] as IResourceWithInput;

        postgres.Inputs["password"].Value.Should().Be("secret_password");
        postgres2.Inputs["password"].Value.Should().Be("other_secret_password");
    }

    [Fact]
    public async Task ExecuteAsync_NonInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true, generatedInputs: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        var postgres = state.LoadedAspireManifestResources["postgrescontainer"] as IResourceWithInput;
        var postgres2 = state.LoadedAspireManifestResources["postgrescontainer2"] as IResourceWithInput;

        postgres.Inputs["password"].Value.Should().HaveLength(20);
        postgres2.Inputs["password"].Value.Should().HaveLength(20);
        postgres.Inputs["password"].Value.Should().NotBe(postgres2.Inputs["password"].Value);
    }

    [Fact]
    public void Validate_NonInteractiveContextSet_ThrowsWhenInputValues()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void Validate_NonInteractiveContextSet_DoesNotThrowWhenOnlyGeneratedSecrets()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true, generatedInputs: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ValidateNonInteractiveState();

        // Assert
        act.Should().NotThrow();
    }

    private static void EnterPasswordInput(TestConsole console, string password)
    {
        // first entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);

        // confirmation entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);
    }
}
