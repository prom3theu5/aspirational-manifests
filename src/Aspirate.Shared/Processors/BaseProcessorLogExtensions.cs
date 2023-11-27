namespace Aspirate.Shared.Processors;

public static class BaseProcessorLogExtensions
{
    public static void LogCreateManifestNotOverridden(this IAnsiConsole console, string processor) =>
        console.MarkupLine($"\t[bold yellow]Processor {processor} has not been configured. CreateManifest must be overridden.[/]");

    public static void LogCompletion(this IAnsiConsole console, string outputPath) =>
        console.MarkupLine($"\t[green]({EmojiLiterals.CheckMark}) Done: [/] Generating [blue]{outputPath}[/]");
}
