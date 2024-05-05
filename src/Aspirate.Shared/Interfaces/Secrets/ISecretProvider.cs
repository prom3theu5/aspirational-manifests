namespace Aspirate.Shared.Interfaces.Secrets;
public interface ISecretProvider
{
    SecretState? State { get; }
    void AddResource(string resourceName);
    bool ResourceExists(string resourceName);
    void RemoveResource(string resourceName);
    bool SecretExists(string resourceName, string key);
    void AddSecret(string resourceName, string key, string value);
    void RemoveSecret(string resourceName, string key);
    void SetState(AspirateState state);
    void LoadState(AspirateState state);
    void RemoveState(AspirateState state);
    bool SecretStateExists(AspirateState state);
    string? GetSecret(string resourceName, string key);
    void SetPassword(string password);
    bool CheckPassword(string password);
}
