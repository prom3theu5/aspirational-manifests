namespace Aspirate.Tests.SecretTests;

public class Base64SecretProviderTests
{
    private const string TestKey = "testKey";
    private const string DecryptedTestValue = "testValue";
    private const string EncryptedTestValue = "dGVzdFZhbHVl";
    private readonly IFileSystem _fileSystem = new MockFileSystem();

    [Fact]
    public void RestoreState_ShouldSetState()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState();
        provider.RestoreState(state);
        provider.State.Should().NotBeNull();
    }

    [Fact]
    public void SaveState_ShouldIncreaseVersion()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState(1);
        provider.RestoreState(state);

        provider.SaveState();

        provider.State.Version.Should().Be(2);
    }

    [Fact]
    public void AddSecret_ShouldAddEncryptedSecretToState()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState();
        provider.RestoreState(state);

        provider.AddSecret(TestKey, DecryptedTestValue);

        provider.State.Secrets.Keys.Should().Contain(TestKey);
    }

    [Fact]
    public void RemoveSecret_ShouldRemoveSecretFromState()
    {
        var provider = new Base64SecretProvider(_fileSystem);

        var state = GetState(
            secrets: new()
            {
                [TestKey] = EncryptedTestValue,
            });

        provider.RestoreState(state);

        provider.RemoveSecret(TestKey);

        provider.State.Secrets.Keys.Should().NotContain(TestKey);
    }

    [Fact]
    public void GetSecret_ShouldReturnDecryptedSecret()
    {
        var provider = new Base64SecretProvider(_fileSystem);

        var state = GetState(
            secrets: new()
            {
                [TestKey] = EncryptedTestValue,
            });

        provider.RestoreState(state);

        var secret = provider.GetSecret(TestKey);

        secret.Should().Be(DecryptedTestValue);
    }

    private static string GetState(int? version = null, Dictionary<string, string>? secrets = null)
    {
        var state = new Base64SecretState
        {
            Version = version ?? 0, Secrets = secrets ?? new Dictionary<string, string>(),
        };

        return JsonSerializer.Serialize(state);
    }
}
