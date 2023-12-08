namespace Aspirate.Tests.ActionsTests.Secrets;

public class SaveSecretsActionTests : BaseActionTests<SaveSecretsAction>
{
    private const string ValidState =
        """
        {
          "salt": "EgEu/M6c1XP/PCkG",
          "hash": "gSeKYq+cBB8Lx1Fw5iuImcUIONz99cQqt6052BjWLp4\u003d",
          "secrets": {
            "postgrescontainer": {
              "ConnectionString_Test": "EgEu/M6c1XP/PCkGUkJTJ9meX9wOz8mY0w0ca46KF3bVqqHah6QLTDwOyTHX"
            },
            "postgrescontainer2": {
              "ConnectionString_Test": "EgEu/M6c1XP/PCkGUkJTJ9meX9wOz8mY0w0ca46KF3bVqqHah6QLTDwOyTHX"
            }
          },
          "secretsVersion": 1
        }
        """;

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

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        console.Output.Should().Contain("Secret State has been saved to");
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
        secretProvider.LoadState();
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
        secretProvider.State.Version.GetValueOrDefault().Should().Be(1);
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
        secretProvider.SetPassword("password_for_secrets");
        secretProvider.SaveState();

        var state = CreateAspirateStateWithConnectionStrings();
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
        console.Input.PushTextWithEnter("password_for_secrets");
        console.Input.PushTextWithEnter("y");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Using existing secrets for provider");
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
        secretProvider.State.Version.GetValueOrDefault().Should().Be(1);
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingDontUseAndDontOverwrite_ShouldAbort()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("password_for_secrets");
        console.Input.PushTextWithEnter("n");
        console.Input.PushTextWithEnter("n");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var state = CreateAspirateStateWithConnectionStrings();
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
        console.Input.PushTextWithEnter("password_for_secrets");

        // dont use existing
        console.Input.PushTextWithEnter("n");

        // overwrite secrets
        console.Input.PushTextWithEnter("y");

        // overwrite secret in postgrescontainer
        console.Input.PushTextWithEnter("y");

        // overwrite secret in postgrescontainer2
        console.Input.PushTextWithEnter("y");

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Secret State has been saved to");
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
        secretProvider.State.Version.GetValueOrDefault().Should().Be(2);
    }
}
