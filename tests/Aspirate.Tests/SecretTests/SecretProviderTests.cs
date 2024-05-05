namespace Aspirate.Tests.SecretTests;

public class SecretProviderTests
{
    private const string TestPassword = "testPassword";
    private const string Base64Salt = "dxaPu37gk4KtgYBy";
    private const string TestKey = "testKey";
    private const string TestResource = "testresource";
    private const string DecryptedTestValue = "testValue";
    private const string EncryptedTestValue = "dxaPu37gk4KtgYByS0Fyt9hQ/dvbURmdavzyWNs8xEgBdduW9Q==";
    private readonly IFileSystem _fileSystem = CreateMockFilesystem();

    [Fact]
    public void SetPassword_ShouldSetHash()
    {
        var provider = new SecretProvider(_fileSystem);
        provider.SetPassword(TestPassword);

        provider.State.Hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CheckPassword_ShouldMatchHash()
    {
        var provider = new SecretProvider(_fileSystem);
        provider.SetPassword(TestPassword);
        provider.CheckPassword(TestPassword).Should().BeTrue();
    }

    [Fact]
    public void SecretState_ShouldExist()
    {
        var provider = new SecretProvider(_fileSystem);
        var state = GetState();
        provider.SecretStateExists(state).Should().BeTrue();
    }

    [Fact]
    public void SecretState_ShouldNotExist()
    {
        var provider = new SecretProvider(_fileSystem);
        var state = new AspirateState();
        provider.SecretStateExists(state).Should().BeFalse();
    }

    [Fact]
    public void RestoreState_ShouldSetState()
    {
        var provider = new SecretProvider(_fileSystem);
        var state = GetState();
        provider.LoadState(state);
        provider.State.Should().NotBeNull();
        provider.State.Salt.Should().BeNull();
    }

    [Fact]
    public void AddSecret_ShouldAddEncryptedSecretToState()
    {
        var provider = new SecretProvider(_fileSystem);
        var state = GetState(Base64Salt);
        provider.LoadState(state);
        provider.SetPassword(TestPassword);

        provider.AddResource(TestResource);
        provider.AddSecret(TestResource, TestKey, DecryptedTestValue);

        provider.State.Secrets[TestResource].Keys.Should().Contain(TestKey);
    }

    [Fact]
    public void RemoveSecret_ShouldRemoveSecretFromState()
    {
        var provider = new SecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
               [TestResource] = new()
               {
                   [TestKey] = EncryptedTestValue,
               },
            });

        provider.LoadState(state);

        provider.SetPassword(TestPassword);

        provider.RemoveSecret(TestResource, TestKey);

        provider.State.Secrets[TestResource].Keys.Should().NotContain(TestKey);
    }

    [Fact]
    public void GetSecret_ShouldReturnDecryptedSecret()
    {
        var provider = new SecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
                [TestResource] = new()
                {
                    [TestKey] = EncryptedTestValue,
                },
            });

        provider.LoadState(state);

        provider.SetPassword(TestPassword);

        var secret = provider.GetSecret(TestResource, TestKey);

        secret.Should().Be(DecryptedTestValue);
    }

    private static AspirateState GetState(string? salt = null, Dictionary<string, Dictionary<string, string>>? secrets = null)
    {
        var state = new SecretState
        {
            Salt = salt,
            Secrets = secrets ?? [],
        };

        return new AspirateState { SecretState = state };
    }

    private static MockFileSystem CreateMockFilesystem()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory($"/some-path/{AspirateLiterals.DefaultArtifactsPath}");
        fileSystem.Directory.SetCurrentDirectory($"/some-path");

        return fileSystem;
    }
}
