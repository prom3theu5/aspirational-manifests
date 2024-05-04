namespace Aspirate.Shared.Interfaces.Secrets;
public interface ISecretProvider
{
    SecretProviderType Type { get; }


    IEncrypter? Encrypter { get; }

    IDecrypter? Decrypter { get; }

    void AddResource(string resourceName);
    bool ResourceExists(string resourceName);
    void RemoveResource(string resourceName);
    bool SecretExists(string resourceName, string key);
    void AddSecret(string resourceName, string key, string value);
    void RemoveSecret(string resourceName, string key);
    void SaveState(string path);
    void LoadState(string path);
    void RemoveState(string path);
    bool SecretStateExists(string path);
    string? GetSecret(string resourceName, string key);
}
