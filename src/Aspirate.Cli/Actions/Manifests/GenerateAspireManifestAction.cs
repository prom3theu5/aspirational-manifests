namespace Aspirate.Cli.Actions.Manifests;

public sealed class GenerateAspireManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        if (!string.IsNullOrEmpty(CurrentState.AspireManifest))
        {
            Logger.MarkupLine($"\r\n[bold]Aspire Manifest supplied at path: [blue]{CurrentState.AspireManifest}[/].[/]");
            Logger.MarkupLine("[bold]Skipping Aspire Manifest generation.[/]");
            Logger.WriteLine();
            return true;
        }

        Logger.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

        var result = await manifestCompositionService.BuildManifestForProject(CurrentState.ProjectPath);

        if (result.Success)
        {
            CurrentState.AspireManifest = result.FullPath;

            Logger.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{CurrentState.AspireManifest}[/]");

            return true;
        }

        Logger.MarkupLine($"[red]Failed to generate Aspire Manifest at: {CurrentState.AspireManifest}[/]");
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }
}
