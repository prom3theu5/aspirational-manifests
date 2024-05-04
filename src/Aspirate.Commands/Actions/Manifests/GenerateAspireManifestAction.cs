namespace Aspirate.Commands.Actions.Manifests;

public sealed class GenerateAspireManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Aspire Manifest[/]");

        if (!string.IsNullOrEmpty(CurrentState.AspireManifest))
        {
            Logger.MarkupLine($"[bold]Aspire Manifest supplied at path: [blue]{CurrentState.AspireManifest}[/].[/]");
            Logger.MarkupLine("[bold]Skipping Aspire Manifest generation.[/]");
            return true;
        }

        Logger.MarkupLine("[bold]Generating Aspire Manifest for supplied App Host:[/]");

        var result = await manifestCompositionService.BuildManifestForProject(CurrentState.ProjectPath);

        if (result.Success)
        {
            CurrentState.AspireManifest = result.FullPath;

            Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{CurrentState.AspireManifest}[/]");

            return true;
        }

        Logger.MarkupLine($"[red]Failed to generate Aspire Manifest at: {CurrentState.AspireManifest}[/]");
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }
}
