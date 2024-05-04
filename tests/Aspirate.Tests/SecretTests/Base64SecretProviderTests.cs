using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Aspirate.Tests.SecretTests;

public class Base64SecretProviderTests
{
    private const string TestKey = "testKey";
    private const string TestResource = "testresource";
    private const string DecryptedTestValue = "testValue";
    private const string EncryptedTestValue = "dGVzdFZhbHVl";
    private readonly IFileSystem _fileSystem = CreateMockFilesystem();

    [Fact]
    public void RestoreState_ShouldSetState()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState();
        WriteStateFile(state);
        provider.LoadState(SecretStoragePath);
        provider.State.Should().NotBeNull();
    }

    [Fact]
    public void SecretState_ShouldExist()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState();
        WriteStateFile(state);
        provider.SecretStateExists(SecretStoragePath).Should().BeTrue();
    }

    [Fact]
    public void SecretState_ShouldNotExist()
    {
        _fileSystem.File.Delete(SecretStoragePath);
        var provider = new Base64SecretProvider(_fileSystem);
        provider.SecretStateExists(SecretStoragePath).Should().BeFalse();
    }

    [Fact]
    public void SaveState_ShouldIncreaseVersion()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState(1);
        WriteStateFile(state);
        provider.LoadState(SecretStoragePath);

        provider.SaveState(SecretStoragePath);

        provider.State.Version.Should().Be(2);
    }

    [Fact]
    public void AddSecret_ShouldAddEncryptedSecretToState()
    {
        var provider = new Base64SecretProvider(_fileSystem);
        var state = GetState();
        WriteStateFile(state);
        provider.LoadState(SecretStoragePath);

        provider.AddResource(TestResource);
        provider.AddSecret(TestResource, TestKey, DecryptedTestValue);

        provider.State.Secrets[TestResource].Keys.Should().Contain(TestKey);
    }

    [Fact]
    public void RemoveSecret_ShouldRemoveSecretFromState()
    {
        var provider = new Base64SecretProvider(_fileSystem);

        var state = GetState(
            secrets: new()
            {
                [TestResource] = new()
                {
                    [TestKey] = EncryptedTestValue,
                },
            });

        WriteStateFile(state);

        provider.LoadState(SecretStoragePath);

        provider.RemoveSecret(TestResource, TestKey);

        provider.State.Secrets[TestResource].Keys.Should().NotContain(TestKey);
    }

    [Fact]
    public void GetSecret_ShouldReturnDecryptedSecret()
    {
        var provider = new Base64SecretProvider(_fileSystem);

        var state = GetState(
            secrets: new()
            {
                [TestResource] = new()
                {
                    [TestKey] = EncryptedTestValue,
                },
            });

        WriteStateFile(state);

        provider.LoadState(SecretStoragePath);

        var secret = provider.GetSecret(TestResource, TestKey);

        secret.Should().Be(DecryptedTestValue);
    }

    private static string GetState(int? version = null, Dictionary<string, Dictionary<string, string>>? secrets = null)
    {
        var state = new Base64SecretState
        {
            Version = version ?? 0,
            Secrets = secrets ?? [],
        };

        return JsonSerializer.Serialize(state);
    }

    private void WriteStateFile(string state) =>
        _fileSystem.File.WriteAllText(SecretStoragePath, state);

    private static string SecretStoragePath => $"/some-path/{AspirateLiterals.DefaultArtifactsPath}/{AspirateLiterals.SecretFileName}";

    private static MockFileSystem CreateMockFilesystem()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory($"/some-path/{AspirateLiterals.DefaultArtifactsPath}");
        fileSystem.Directory.SetCurrentDirectory($"/some-path");

        return fileSystem;
    }
}
