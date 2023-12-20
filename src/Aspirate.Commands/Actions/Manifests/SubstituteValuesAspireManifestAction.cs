namespace Aspirate.Commands.Actions.Manifests;

public class SubstituteValuesAspireManifestAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        ReplacePlaceholdersInParsedResources(CurrentState.LoadedAspireManifestResources);

        return Task.FromResult(true);
    }

    private void ReplacePlaceholdersInParsedResources(Dictionary<string, Resource> resources)
    {
        var containers = resources.Where(x => x.Value.Type == AspireComponentLiterals.Container);
        var nonContainers = resources.Where(x => x.Value.Type != AspireComponentLiterals.Container);

        ProcessPlaceholders(resources, containers);
        ProcessPlaceholders(resources, nonContainers);
    }

    private void ProcessPlaceholders(Dictionary<string, Resource> resources, IEnumerable<KeyValuePair<string, Resource>> resourcesToProcess)
    {
        foreach (var resourceEntry in resourcesToProcess)
        {
            var resourceProcessor = Services.GetKeyedService<IResourceProcessor>(resourceEntry.Value.Type);
            resourceProcessor?.ReplacePlaceholders(resourceEntry.Value, resources);
        }
    }
}
