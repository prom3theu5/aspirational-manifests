namespace Aspirate.Contracts.Models.State;

public class ComputedParametersState
{
    public string? ActiveKubernetesContext { get; set; }
    public string? AspireManifestPath { get; private set; }
    public string? KustomizeManifestPath { get; private set; }
    public List<string> AspireComponentsToProcess { get; private set; } = [];
    public Dictionary<string, Resource> LoadedAspireManifestResources { get; private set; } = [];
    public Dictionary<string, Resource> FinalResources { get; } = [];

    public List<KeyValuePair<string, Resource>> SelectedProjectComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is Project && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    public List<KeyValuePair<string, Resource>> AllSelectedSupportedComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is not UnsupportedResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    public bool ActiveKubernetesContextIsSet => !string.IsNullOrEmpty(ActiveKubernetesContext);

    public void SetAspireManifestPath(string path) => AspireManifestPath = path;

    public void SetKustomizeManifestPath(string path) => KustomizeManifestPath = path;

    public void SetAspireComponentsToProcess(List<string> components) => AspireComponentsToProcess = components;

    public void SetLoadedManifestState(Dictionary<string, Resource> resources) => LoadedAspireManifestResources = resources;

    public void AppendToFinalResources(string key, Resource resource) => FinalResources.Add(key, resource);
}
