namespace Aspirate.Cli.Actions.Manifests;

public class LoadAspireManifestAction(
    IManifestFileParserService manifestFileParserService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveSupport(serviceProvider)
{
    public const string ActionKey = "LoadAspireManifestAction";

    public override Task<bool> ExecuteAsync()
    {
        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(CurrentState.ProjectManifest);
        CurrentState.LoadedAspireManifestResources = aspireManifest;

        var componentsToProcess = SelectManifestItemsToProcess();
        CurrentState.AspireComponentsToProcess = componentsToProcess;

        return Task.FromResult(true);
    }

    private List<string> SelectManifestItemsToProcess()
    {
        if (CurrentState.NonInteractive)
        {
            Logger.MarkupLine("\r\n[blue]Non-Interactive Mode: Processing all components in the loaded file.[/]\r\n");
            return CurrentState.LoadedAspireManifestResources.Keys.ToList();
        }

        return Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]components[/] to process from the loaded file")
                .PageSize(10)
                .Required()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.LoadedAspireManifestResources.Keys.ToList()));
    }

    public override void ValidateNonInteractiveState()
    {
        if (string.IsNullOrEmpty(CurrentState.ProjectManifest))
        {
            NonInteractiveValidationFailed("No Aspire Manifest file was supplied.");
        }
    }
}
