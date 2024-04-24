namespace Aspirate.Services.Interfaces;

public interface IKustomizeService
{
    Task<string> RenderManifestUsingKustomize(string kustomizePath);
    Task WriteSecretsOutToTempFiles(bool disableSecrets, string inputPath, List<string> files, ISecretProvider secretProvider);
    void CleanupSecretEnvFiles(bool disableSecrets, IEnumerable<string> secretFiles);
    CommandAvailableResult IsKustomizeAvailable();
}
