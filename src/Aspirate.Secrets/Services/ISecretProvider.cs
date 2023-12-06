namespace Aspirate.Secrets.Services;
public interface ISecretProvider
{
    ProviderType Type { get; }

    IEncrypter? Encrypter { get; }

    IDecrypter? Decrypter { get; }

    void AddSecret(string key, string value);
    void RemoveSecret(string key);
    void RestoreState(string state);
    void SaveState(string? path = null);
    string? GetSecret(string key);
}
