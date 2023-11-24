namespace Aspirate.Contracts.Interfaces;

public interface IKubeCtlService
{
    Task<bool> SelectKubernetesContextForDeployment();
    Task<bool> ApplyManifests(string outputFolder);
    Task<bool> RemoveManifests(string outputFolder);
    string GetActiveContextName();
}
