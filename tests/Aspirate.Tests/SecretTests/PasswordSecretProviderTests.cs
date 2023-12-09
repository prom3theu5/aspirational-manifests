using Aspirate.Secrets.Literals;

namespace Aspirate.Tests.SecretTests;

public class PasswordSecretProviderTests
{
    private const string TestPassword = "testPassword";
    private const string Base64Salt = "dxaPu37gk4KtgYBy";
    private const string TestKey = "testKey";
    private const string TestResource = "testresource";
    private const string DecryptedTestValue = "testValue";
    private const string EncryptedTestValue = "dxaPu37gk4KtgYByS0Fyt9hQ/dvbURmdavzyWNs8xEgBdduW9Q==";
    private readonly IFileSystem _fileSystem = new MockFileSystem();

    [Fact]
    public void SetPassword_ShouldInitializeEncrypterAndDecrypter()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        provider.SetPassword(TestPassword);

        provider.Encrypter.Should().NotBeNull();
        provider.Decrypter.Should().NotBeNull();
    }

    [Fact]
    public void SetPassword_ShouldSetHash()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        provider.SetPassword(TestPassword);

        provider.Encrypter.Should().NotBeNull();
        provider.State.Hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CheckPassword_ShouldMatchHash()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        provider.SetPassword(TestPassword);
        provider.CheckPassword(TestPassword).Should().BeTrue();
    }

    [Fact]
    public void SecretState_ShouldExist()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState();
        WriteStateFile(state);
        provider.SecretStateExists().Should().BeTrue();
    }

    [Fact]
    public void SecretState_ShouldNotExist()
    {
        _fileSystem.File.Delete(_fileSystem.Path.Combine("/", AspirateSecretLiterals.SecretsStateFile));
        var provider = new PasswordSecretProvider(_fileSystem);
        provider.SecretStateExists().Should().BeFalse();
    }

    [Fact]
    public void RestoreState_ShouldSetState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState();
        WriteStateFile(state);
        provider.LoadState("/");
        provider.State.Should().NotBeNull();
        provider.State.Salt.Should().BeNull();
    }

    [Fact]
    public void SaveState_ShouldIncreaseVersion()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState(Base64Salt, 1);
        WriteStateFile(state);
        provider.LoadState("/");

        provider.SaveState();

        provider.State.Version.Should().Be(2);
    }

    [Fact]
    public void AddSecret_ShouldAddEncryptedSecretToState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState(Base64Salt);
        WriteStateFile(state);
        provider.LoadState("/");
        provider.SetPassword(TestPassword);

        provider.AddResource(TestResource);
        provider.AddSecret(TestResource, TestKey, DecryptedTestValue);

        provider.State.Secrets[TestResource].Keys.Should().Contain(TestKey);
    }

    [Fact]
    public void RemoveSecret_ShouldRemoveSecretFromState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
               [TestResource] = new()
               {
                   [TestKey] = EncryptedTestValue,
               },
            });

        WriteStateFile(state);
        provider.LoadState("/");

        provider.SetPassword(TestPassword);

        provider.RemoveSecret(TestResource, TestKey);

        provider.State.Secrets[TestResource].Keys.Should().NotContain(TestKey);
    }

    [Fact]
    public void GetSecret_ShouldReturnDecryptedSecret()
    {
        var provider = new PasswordSecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
                [TestResource] = new()
                {
                    [TestKey] = EncryptedTestValue,
                },
            });

        WriteStateFile(state);
        provider.LoadState("/");

        provider.SetPassword(TestPassword);

        var secret = provider.GetSecret(TestResource, TestKey);

        secret.Should().Be(DecryptedTestValue);
    }

    private static string GetState(string? salt = null, int? version = null, Dictionary<string, Dictionary<string, string>>? secrets = null)
    {
        var state = new PasswordSecretState
        {
            Salt = salt,
            Version = version ?? 0,
            Secrets = secrets ?? [],
        };

        return JsonSerializer.Serialize(state);
    }

    private void WriteStateFile(string state)
    {
        var outputFile = _fileSystem.Path.Combine("/", AspirateSecretLiterals.SecretsStateFile);
        _fileSystem.File.WriteAllText(outputFile, state);
    }
}
