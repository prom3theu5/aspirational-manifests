using System;
using System.Threading.Tasks;
using Xunit;

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

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings();
        state.SecretState = JsonSerializer.Deserialize<SecretState>(ValidState);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();
        secretProvider.State.Secrets.Count.Should().Be(4);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
        secretProvider.LoadState(state);
        secretProvider.State.Secrets.Count.Should().Be(4);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
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

        var mockFilesystem = new MockFileSystem();
        var secretProvider = new SecretProvider(mockFilesystem);
        var state = CreateAspirateStateWithConnectionStrings();
        state.SecretState = JsonSerializer.Deserialize<SecretState>(ValidState);
        secretProvider.SetPassword("password_for_secrets");
        secretProvider.SetState(state);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: mockFilesystem);
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

        // Existing password
        console.Input.PushTextWithEnter("password_for_secrets");

        // Select Use Existing
        console.Input.PushKey(ConsoleKey.Enter);

        var state = CreateAspirateStateWithConnectionStrings();
        state.SecretState = JsonSerializer.Deserialize<SecretState>(ValidState);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);
        var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Secret State has been saved");
        secretProvider.State.Secrets.Count.Should().Be(4);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingDontUseAndOverwrite_ShouldBeSuccess()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        // Existing password
        console.Input.PushTextWithEnter("password_for_secrets");
        // Select Overwrite
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Enter);

        // New password
        console.Input.PushTextWithEnter("password_for_secrets");

        // Confirm new password
        console.Input.PushTextWithEnter("password_for_secrets");

        var state = CreateAspirateStateWithConnectionStrings();
        state.SecretState = JsonSerializer.Deserialize<SecretState>(ValidState);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);
        var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Secret State has been saved");
        secretProvider.State.Secrets.Count.Should().Be(4);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
    }

    [Fact]
    public async Task SaveSecretsAction_WhenExistingAugment_ShouldBeSuccess()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        // Existing password

        console.Input.PushTextWithEnter("password_for_secrets");

        // Select Augment
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Enter);

        // Select Replace
        console.Input.PushKey(ConsoleKey.DownArrow);
        console.Input.PushKey(ConsoleKey.Enter);

        // Select Use Existing
        console.Input.PushKey(ConsoleKey.Enter);

        var state = CreateAspirateStateWithConnectionStrings();
        state.SecretState = JsonSerializer.Deserialize<SecretState>(ValidState);
        var serviceProvider = CreateServiceProvider(state, console);
        var secretProvider = serviceProvider.GetRequiredService<ISecretProvider>();
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ExecuteAsync();

        // Assert
        await act.Should().NotThrowAsync();
        console.Output.Should().Contain("Secret State has been saved");
        secretProvider.State.Secrets.Count.Should().Be(4);
        secretProvider.State.Secrets["postgrescontainer"].Count.Should().Be(1);
        secretProvider.State.Secrets["postgrescontainer2"].Count.Should().Be(1);
    }
}
