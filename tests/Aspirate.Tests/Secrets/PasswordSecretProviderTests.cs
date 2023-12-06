namespace Aspirate.Tests.Secrets;

public class PasswordSecretProviderTests
{
    private const string TestPassword = "testPassword";
    private const string Base64Salt = "dxaPu37gk4KtgYBy";
    private const string TestKey = "testKey";
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
    public void RestoreState_ShouldSetState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState();
        provider.RestoreState(state);
        provider.State.Should().NotBeNull();
        provider.State.Salt.Should().BeNull();
    }

    [Fact]
    public void SaveState_ShouldIncreaseVersion()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState(Base64Salt, 1);
        provider.RestoreState(state);

        provider.SaveState();

        provider.State.Version.Should().Be(2);
    }

    [Fact]
    public void AddSecret_ShouldAddEncryptedSecretToState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);
        var state = GetState(Base64Salt);
        provider.RestoreState(state);
        provider.SetPassword(TestPassword);

        provider.AddSecret(TestKey, DecryptedTestValue);

        provider.State.Secrets.Keys.Should().Contain(TestKey);
    }

    [Fact]
    public void RemoveSecret_ShouldRemoveSecretFromState()
    {
        var provider = new PasswordSecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
                [TestKey] = EncryptedTestValue,
            });

        provider.RestoreState(state);

        provider.SetPassword(TestPassword);

        provider.RemoveSecret(TestKey);

        provider.State.Secrets.Keys.Should().NotContain(TestKey);
    }

    [Fact]
    public void GetSecret_ShouldReturnDecryptedSecret()
    {
        var provider = new PasswordSecretProvider(_fileSystem);

        var state = GetState(
            Base64Salt, secrets: new()
            {
                [TestKey] = EncryptedTestValue,
            });

        provider.RestoreState(state);

        provider.SetPassword(TestPassword);

        var secret = provider.GetSecret(TestKey);

        secret.Should().Be(DecryptedTestValue);
    }

    private static string GetState(string? salt = null, int? version = null, Dictionary<string, string>? secrets = null)
    {
        var state = new PasswordSecretState
        {
            Salt = salt,
            Version = version ?? 0,
            Secrets = secrets ?? [],
        };

        return JsonSerializer.Serialize(state);
    }
}
