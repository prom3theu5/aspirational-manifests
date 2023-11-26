namespace Aspirate.Cli.Actions.Manifests;

public sealed class GenerateAspireManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "GenerateAspireManifestAction";

    public override async Task<bool> ExecuteAsync()
    {
        Logger.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

        var result = await manifestCompositionService.BuildManifestForProject(CurrentState.ProjectPath);

        if (result.Success)
        {
            CurrentState.ProjectManifest = result.FullPath;

            Logger.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{CurrentState.ProjectManifest}[/]");

            return true;
        }

        Logger.MarkupLine($"[red]Failed to generate Aspire Manifest at: {CurrentState.ProjectManifest}[/]");
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }
}
