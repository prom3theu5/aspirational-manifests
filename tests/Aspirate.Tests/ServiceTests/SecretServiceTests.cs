using Aspirate.Secrets;

namespace Aspirate.Tests.ServiceTests;

public class SecretServiceTests : BaseServiceTests<ISecretService>
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
    public void LoadState_NotExists_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var fileSystem = new MockFileSystem();

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = false,
            DisableSecrets = false,
            SecretPassword = string.Empty,
        });

        // Assert
        state.SecretState.Secrets.Count.Should().Be(0);
    }

    [Fact]
    public void  LoadState_ExistingState_Success()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        console.Input.PushTextWithEnter("password_for_secrets");

        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(SecretStoragePath, ValidState);

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings();
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = false,
            DisableSecrets = false,
            SecretPassword = string.Empty,
        });

        // Assert
        secretProvider.State.Secrets.Count.Should().Be(2);
        secretProvider.State.Secrets.ElementAt(0).Value.Count.Should().Be(1);
        secretProvider.State.Secrets.ElementAt(1).Value.Count.Should().Be(1);
    }

    [Fact]
    public void LoadState_ExistingStateNonInteractive_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(SecretStoragePath, ValidState);

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true, password: "password_for_secrets");
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = true,
            DisableSecrets = false,
            SecretPassword = state.SecretPassword,
        });

        // Assert
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
        fileSystem.AddFile(SecretStoragePath, ValidState);

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = true,
            DisableSecrets = false,
            SecretPassword = string.Empty,
        });;

        // Assert
        result.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void LoadState_NonInteractiveInvalidPasswordThrows_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(SecretStoragePath, ValidState);

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true, password: "invalid_password");
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = true,
            DisableSecrets = false,
            SecretPassword = string.Empty,
        });;

        // Assert
        result.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void LoadState_NoState_Success()
    {
        // Arrange
        var console = new TestConsole();
        var fileSystem = new MockFileSystem();

        var secretProvider = new SecretProvider(fileSystem);

        var state = CreateAspirateStateWithConnectionStrings(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console, secretProvider: secretProvider, fileSystem: fileSystem);
        var service = GetSystemUnderTest(serviceProvider);

        // Act
        var result = () => service.LoadSecrets(new SecretManagementOptions
        {
            State = state,
            NonInteractive = true,
            DisableSecrets = false,
            SecretPassword = string.Empty,
        });;

        // Assert
        result.Should().NotThrow();
    }

    private static string SecretStoragePath => $"/some-path/aspirate-output/{AspirateLiterals.SecretFileName}";
}
