namespace Aspirate.Cli.Actions.Manifests;

public class LoadAspireManifestAction(
    IManifestFileParserService manifestFileParserService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "LoadAspireManifestAction";

    public override Task<bool> ExecuteAsync()
    {
        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(CurrentState.ComputedParameters.AspireManifestPath);
        CurrentState.ComputedParameters.SetLoadedManifestState(aspireManifest);

        var componentsToProcess = SelectManifestItemsToProcess();
        CurrentState.ComputedParameters.SetAspireComponentsToProcess(componentsToProcess);

        return Task.FromResult(true);
    }

    private List<string> SelectManifestItemsToProcess() =>
        Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]components[/] to process from the loaded file")
                .PageSize(10)
                .Required()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.ComputedParameters.LoadedAspireManifestResources.Keys.ToList()));
}
