namespace Aspirate.Shared.Interfaces.Services;

public interface IKustomizeService
{
    Task<string> RenderManifestUsingKustomize(string kustomizePath);
    Task WriteSecretsOutToTempFiles(AspirateState state, List<string> files, ISecretProvider secretProvider);
    void CleanupSecretEnvFiles(bool disableSecrets, IEnumerable<string> secretFiles);
    CommandAvailableResult IsKustomizeAvailable();
}
