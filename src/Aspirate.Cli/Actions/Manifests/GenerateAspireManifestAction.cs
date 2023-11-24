namespace Aspirate.Cli.Actions.Manifests;

public sealed class GenerateAspireManifestAction(
    IAspireManifestCompositionService manifestCompositionService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "GenerateAspireManifestAction";

    public override async Task<bool> ExecuteAsync()
    {
        Logger.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

        var result = await manifestCompositionService.BuildManifestForProject(CurrentState.InputParameters.AspireManifestPath);

        if (result.Success)
        {
            CurrentState.ComputedParameters.SetAspireManifestPath(result.FullPath);

            Logger.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{result.FullPath}[/]");

            return true;
        }

        Logger.MarkupLine($"[red]Failed to generate Aspire Manifest at: {result.FullPath}[/]");
        throw new InvalidOperationException("Failed to generate Aspire Manifest.");
    }
}
