namespace Aspirate.Commands.Actions.Manifests;

public class LoadAspireManifestAction(
    IManifestFileParserService manifestFileParserService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Selecting Required Components[/]");

        var aspireManifest = manifestFileParserService.LoadAndParseAspireManifest(CurrentState.AspireManifest);
        CurrentState.LoadedAspireManifestResources = aspireManifest;

        SelectManifestItemsToProcess();

        return Task.FromResult(true);
    }

    private void SelectManifestItemsToProcess()
    {
        if (CurrentState.NonInteractive)
        {
            if (CurrentState.HasSelectedComponentsInCli)
            {
                Logger.MarkupLine("[green]Processing components that were specified in CLI[/]");

                var componentsInManifest = new HashSet<string>([.. CurrentState.LoadedAspireManifestResources.Keys]);
                var cliComponentsNotInManifest = new HashSet<string>(CurrentState.CliSpecifiedComponents.Where(c => !componentsInManifest.Contains(c)));

                if (cliComponentsNotInManifest.Any())
                {
                    Logger.MarkupLine($"[yellow]Some components specified in CLI were not found in the Aspire manifest. Will not process these:[/]");
                    foreach (string cliComponent in cliComponentsNotInManifest)
                    {
                        Logger.MarkupLine($"[yellow]{cliComponent}[/]");
                    }

                    CurrentState.CliSpecifiedComponents.RemoveAll(cliComponentsNotInManifest.Contains);
                }

                CurrentState.AspireComponentsToProcess = CurrentState.LoadedAspireManifestResources.Keys
                    .Where(CurrentState.CliSpecifiedComponents.Contains).ToList();

                Logger.MarkupLine("[green]Components that will be processed:[/]");
                foreach (string component in CurrentState.AspireComponentsToProcess)
                {
                    Logger.MarkupLine($"[blue]{component}[/]");
                }
                return;
            }
            else
            {
                Logger.MarkupLine("[blue]Non-Interactive Mode: No components were specified. Processing all components in the loaded file.[/]");
                CurrentState.AspireComponentsToProcess = CurrentState.LoadedAspireManifestResources.Keys.ToList();
                return;
            }
        }

        if (CurrentState.ProcessAllComponents == true)
        {
            Logger.MarkupLine("[blue]Processing all components in the loaded file, as per the state file.[/]");
            CurrentState.AspireComponentsToProcess = CurrentState.LoadedAspireManifestResources.Keys.ToList();
            return;
        }

        var componentsToProcess = Logger.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]components[/] to process from the loaded file")
                .PageSize(10)
                .Required()
                .MoreChoicesText("[grey](Move up and down to reveal more components)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle a component, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("All Components", CurrentState.LoadedAspireManifestResources.Keys.ToList()));

        CurrentState.AspireComponentsToProcess = componentsToProcess;

        if (componentsToProcess.Count == CurrentState.LoadedAspireManifestResources.Count)
        {
            CurrentState.ProcessAllComponents = true;
        }
    }

    public override void ValidateNonInteractiveState()
    {
        if (string.IsNullOrEmpty(CurrentState.AspireManifest))
        {
            Logger.ValidationFailed("No Aspire Manifest file was supplied.");
        }
    }
}
