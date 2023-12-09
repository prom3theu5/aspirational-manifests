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
        foreach (var resourceEntry in resources)
        {
            var processor = Services.GetKeyedService<IProcessor>(resourceEntry.Value.Type);
            processor?.ReplacePlaceholders(resourceEntry.Value, resources);
        }
    }
}
