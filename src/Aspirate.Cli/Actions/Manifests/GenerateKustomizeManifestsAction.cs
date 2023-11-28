namespace Aspirate.Cli.Actions.Manifests;

public sealed class GenerateKustomizeManifestsAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
         if (NoSupportedComponentsExitAction())
         {
             return true;
         }

         Logger.MarkupLine("\r\n[bold]Generating kustomize manifests to run against your kubernetes cluster:[/]\r\n");

         foreach (var resource in CurrentState.AllSelectedSupportedComponents)
         {
             await ProcessIndividualResourceManifests(resource);
         }

         return true;
    }

    private bool NoSupportedComponentsExitAction()
    {
        if (CurrentState.HasSelectedSupportedComponents)
        {
            return false;
        }

        Logger.MarkupLine("\r\n[bold]No supported components selected. Skipping generation of kustomize manifests.[/]");
        return true;
    }

    private async Task ProcessIndividualResourceManifests(KeyValuePair<string, Resource> resource)
    {
        if (resource.Value.Type is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unknown.[/]");
            return;
        }

        var handler = Services.GetKeyedService<IProcessor>(resource.Value.Type);

        if (handler is null)
        {
            Logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return;
        }

        var success = await handler.CreateManifests(resource, CurrentState.OutputPath, CurrentState.TemplatePath);

        if (success && !CurrentState.IsDatabase(resource.Value))
        {
            CurrentState.AppendToFinalResources(resource.Key, resource.Value);
        }
    }
}
