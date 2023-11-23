namespace Aspirate.Cli.Commands.EndToEnd;

public partial class EndToEndCommand
{
    private static void LogGeneratingManifests() =>
        AnsiConsole.MarkupLine("\r\n[bold]Generating kustomize manifests to run against your kubernetes cluster:[/]\r\n");

    private static void LogGeneratingAspireManifest() =>
        AnsiConsole.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

    private static void LogBuildingAndPushingContainers() =>
        AnsiConsole.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

    private static void LogTypeUnknown(string resourceName) =>
        AnsiConsole.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unknown.[/]");

    private static void LogUnsupportedType(string resourceName) =>
        AnsiConsole.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unsupported.[/]");

    private static Task LogContainerCompositionCompleted()
    {
        AnsiConsole.MarkupLine("\r\n[bold]Generation completed.[/]");
        return Task.Delay(2000);
    }

    private static void LogGatheringContainerDetailsFromProjects() =>
        AnsiConsole.MarkupLine("\r\n[bold]Gathering container details for each project in selected components[/]\r\n");

    private static Task LogGatheringContainerDetailsFromProjectsCompleted()
    {
        AnsiConsole.MarkupLine("\r\n[bold]Gathering Tasks Completed - Cache Populated.[/]");
        return Task.Delay(2000);
    }

    private static Task LogCreatedManifestAtPath(string resultFullPath)
    {
        AnsiConsole.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{resultFullPath}[/]");
        return Task.Delay(2000);
    }

    private static void LogCommandCompleted() =>
        AnsiConsole.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed - Happy Deployment {EmojiLiterals.Smiley}[/]");
}
