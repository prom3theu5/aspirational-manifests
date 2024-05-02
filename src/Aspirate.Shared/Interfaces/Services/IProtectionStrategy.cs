namespace Aspirate.Shared.Interfaces.Services;

public interface ISecretProtectionStrategy
{
    bool HasSecrets(KeyValuePair<string, Resource> component);
    void ProtectSecrets(KeyValuePair<string, Resource> component, bool nonInteractive);
}
