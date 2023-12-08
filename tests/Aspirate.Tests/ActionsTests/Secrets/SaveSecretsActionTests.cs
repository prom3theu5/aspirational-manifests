namespace Aspirate.Tests.ActionsTests.Secrets;

public class SaveSecretsActionTests : BaseActionTests<SaveSecretsAction>
{
    [Fact]
    public async Task SaveSecretsAction_SaveSecrets_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("password_for_secrets");
        console.Input.PushTextWithEnter("password_for_secrets");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithInputs(passwordsSet: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        console.Output.Should().Contain("Secret State has been saved to");
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingIncorrectPassword_ShouldAbort()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("incorrect_password");
        console.Input.PushTextWithEnter("incorrect_password");
        console.Input.PushTextWithEnter("incorrect_password");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        secretProvider.SetPassword("correct_password");
        secretProvider.SaveState();

        var state = CreateAspirateStateWithInputs(passwordsSet: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);


        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().ThrowAsync<ActionCausesExitException>();
        console.Output.Should().Contain("Aborting due to inability to unlock secrets.");
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingAndCorrectPassword_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("correct_password");
        console.Input.PushTextWithEnter("y");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        secretProvider.SetPassword("correct_password");
        secretProvider.SaveState();

        var state = CreateAspirateStateWithInputs(passwordsSet: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Using existing secrets for provider");
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingDontUseAndDontOverwrite_ShouldAbort()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("correct_password");
        console.Input.PushTextWithEnter("n");
        console.Input.PushTextWithEnter("n");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        secretProvider.SetPassword("correct_password");
        secretProvider.SaveState();

        var state = CreateAspirateStateWithInputs(passwordsSet: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().ThrowAsync<ActionCausesExitException>();
        console.Output.Should().Contain("Aborting due to inability to modify secrets");
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingDontUseAndOverwrite_ShouldBeSuccess()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("correct_password");
        console.Input.PushTextWithEnter("n");
        console.Input.PushTextWithEnter("y");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        secretProvider.SetPassword("correct_password");
        secretProvider.SaveState();

        var state = CreateAspirateStateWithInputs(passwordsSet: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Secret State has been saved to");
    }
}
