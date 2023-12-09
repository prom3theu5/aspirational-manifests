namespace Aspirate.Tests.ActionsTests.Secrets;

public class LoadSecretsActionTests : BaseActionTests<LoadSecretsAction>
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
    public async Task LoadState_NotExists_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task LoadState_ExistingState_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("password_for_secrets");

        var fileSystem = new MockFileSystem();
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act

        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets.ElementAt(0).Value.Count.Should().Be(1);
        secretProvider.State.Secrets.ElementAt(1).Value.Count.Should().Be(1);
    }

    [Fact]
    public async Task LoadState_ExistingStateNonInteractive_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true, password: "password_for_secrets");
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        action.ValidateNonInteractiveState();
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets.ElementAt(0).Value.Count.Should().Be(1);
        secretProvider.State.Secrets.ElementAt(1).Value.Count.Should().Be(1);
    }

    [Fact]
    public void LoadState_NonInteractiveNoPasswordThrows_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => action.ValidateNonInteractiveState();

        // Assert
        result.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void LoadState_NonInteractiveInvalidPasswordThrows_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile($"/{AspirateSecretLiterals.SecretsStateFile}", ValidState);

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true, password: "invalid_password");
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => action.ValidateNonInteractiveState();

        // Assert
        result.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void LoadState_NoState_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();

        var secretProvider = new PasswordSecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => action.ValidateNonInteractiveState();

        // Assert
        result.Should().NotThrow();
    }
}
