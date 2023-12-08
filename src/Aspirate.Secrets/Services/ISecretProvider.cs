namespace Aspirate.Secrets.Services;
public interface ISecretProvider
{
    ProviderType Type { get; }

    IEncrypter? Encrypter { get; }

    IDecrypter? Decrypter { get; }

    void AddResource(string resourceName);
    void RemoveResource(string resourceName);
    void AddSecret(string resourceName, string key, string value);
    void RemoveSecret(string resourceName, string key);
    void SaveState(string? path = null);
    void LoadState(string? path = null);
    bool SecretStateExists(string? path = null);
    string? GetSecret(string resourceName, string key);
}
