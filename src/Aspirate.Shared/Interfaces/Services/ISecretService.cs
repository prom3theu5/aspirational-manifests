namespace Aspirate.Shared.Interfaces.Services;

public interface ISecretService
{
    void LoadSecrets(SecretManagementOptions options);
    void SaveSecrets(SecretManagementOptions options);
    void ReInitialiseSecrets(SecretManagementOptions options);
}
