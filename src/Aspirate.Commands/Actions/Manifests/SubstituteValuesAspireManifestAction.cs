namespace Aspirate.Commands.Actions.Manifests;

public class SubstituteValuesAspireManifestAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Value and Parameter Substitution[/]");

        ReplacePlaceholdersInParsedResources(CurrentState.LoadedAspireManifestResources);

        return Task.FromResult(true);
    }

    private void ReplacePlaceholdersInParsedResources(Dictionary<string, Resource> resources)
    {
        var containers = resources.Where(x => x.Value.Type == AspireComponentLiterals.Container);
        var projects = resources.Where(x => x.Value.Type == AspireComponentLiterals.Project);
        var nonContainersOrProjects = resources.Where(x => x.Value.Type != AspireComponentLiterals.Container && x.Value.Type != AspireComponentLiterals.Project);

        ProcessPlaceholders(resources, containers);
        ProcessPlaceholders(resources, nonContainersOrProjects);
        ProcessPlaceholders(resources, projects);
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
