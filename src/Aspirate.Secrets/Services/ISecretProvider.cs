namespace Aspirate.Secrets.Services;
public interface ISecretProvider
{
    ProviderType Type { get; }

    IEncrypter? Encrypter { get; }

    IDecrypter? Decrypter { get; }

    void AddResource(string resourceName);
    bool ResourceExists(string resourceName);
    void RemoveResource(string resourceName);
    bool SecretExists(string resourceName, string key);
    void AddSecret(string resourceName, string key, string value);
    void RemoveSecret(string resourceName, string key);
    void SaveState(string? path = null);
    void LoadState(string? path = null);
    void RemoveState(string? path = null);
    bool SecretStateExists(string? path = null);
    string? GetSecret(string resourceName, string key);
}
