namespace Aspirate.Shared.Interfaces.Services;

public interface IKubernetesService
{
    List<object> CreateDashboardKubernetesObjects();
    List<object> ConvertResourcesToKubeObjects(List<KeyValuePair<string, Resource>> supportedResources, AspirateState state, bool encodeSecrets);
    Task InteractivelySelectKubernetesCluster(AspirateState state);
    Task<bool> IsNamespaceEmpty(KubernetesRunOptions options);
    Task<bool> ClearNamespace(KubernetesRunOptions options);
    Task<bool> DeleteNamespace(KubernetesRunOptions options);
    Task<bool> ApplyObjectsToCluster(KubernetesRunOptions options);
    Task ListServiceAddresses(KubernetesRunOptions options);
    IKubernetes CreateClient(string clusterName);
}
