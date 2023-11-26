namespace Aspirate.Shared.Models.State;

public class AspirateState
{
    public string? ProjectPath { get; set; }
    public string? ProjectManifest { get; set; }
    public string? InputPath { get; set; }
    public string? OutputPath { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }
    public string? TemplatePath { get; set; }
    public string? KubeContext { get; set; }
    public bool NonInteractive { get; set; }
    public bool SkipBuild { get; set; }
    public List<string> AspireComponentsToProcess { get; set; } = [];
    public Dictionary<string, Resource> LoadedAspireManifestResources { get; set; } = [];
    public Dictionary<string, Resource> FinalResources { get; } = [];
    public bool ActiveKubernetesContextIsSet => !string.IsNullOrEmpty(KubeContext);
    public List<KeyValuePair<string, Resource>> SelectedProjectComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is Project && AspireComponentsToProcess.Contains(x.Key))
            .ToList();
    public List<KeyValuePair<string, Resource>> AllSelectedSupportedComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is not UnsupportedResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();
    public void AppendToFinalResources(string key, Resource resource) =>
        FinalResources.Add(key, resource);
}
