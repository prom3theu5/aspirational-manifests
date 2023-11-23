namespace Aspirate.Cli.Commands.EndToEnd;

public static class EndToEndLogExtensions
{
    public static void LogGeneratingManifests(this IAnsiConsole console) =>
        console.MarkupLine("\r\n[bold]Generating kustomize manifests to run against your kubernetes cluster:[/]\r\n");

    public static void LogGeneratingAspireManifest(this IAnsiConsole console) =>
        console.MarkupLine("\r\n[bold]Generating Aspire Manifest for supplied App Host:[/]\r\n");

    public static void LogBuildingAndPushingContainers(this IAnsiConsole console) =>
        console.MarkupLine("\r\n[bold]Building all project resources, and pushing containers:[/]\r\n");

    public static void LogTypeUnknown(this IAnsiConsole console, string resourceName) =>
        console.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unknown.[/]");

    public static void LogUnsupportedType(this IAnsiConsole console, string resourceName) =>
        console.MarkupLine($"[yellow]Skipping resource '{resourceName}' as its type is unsupported.[/]");

    public static Task LogContainerCompositionCompleted(this IAnsiConsole console)
    {
        console.MarkupLine("\r\n[bold]Generation completed.[/]");
        return Task.Delay(2000);
    }

    public static void LogGatheringContainerDetailsFromProjects(this IAnsiConsole console) =>
        console.MarkupLine("\r\n[bold]Gathering container details for each project in selected components[/]\r\n");

    public static Task LogGatheringContainerDetailsFromProjectsCompleted(this IAnsiConsole console)
    {
        console.MarkupLine("\r\n[bold]Gathering Tasks Completed - Cache Populated.[/]");
        return Task.Delay(2000);
    }

    public static Task LogCreatedManifestAtPath(this IAnsiConsole console, string resultFullPath)
    {
        console.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Created Aspire Manifest At Path: [blue]{resultFullPath}[/]");
        return Task.Delay(2000);
    }

    public static void LogCommandCompleted(this IAnsiConsole console) =>
        console.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed - Happy Deployment {EmojiLiterals.Smiley}[/]");

    public static void LogLoadedConfigurationFile(this IAnsiConsole console, string pathToFile) =>
        console.MarkupLine($"\r\n[bold] Successfully loaded existing aspirate bootstrap settings from [blue]'{pathToFile}'[/].[/]");

    public static void LogFailedToGenerateAspireManifest(this IAnsiConsole console, string path) =>
        console.MarkupLine($"[red]Failed to generate Aspire Manifest at: {path}[/]");
}
