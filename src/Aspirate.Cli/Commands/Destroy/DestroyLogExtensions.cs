namespace Aspirate.Cli.Commands.Destroy;

public static class DestroyLogExtensions
{
    public static void LogDestroyCommandCompleted(this IAnsiConsole console, string cluster) =>
        console.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments removed from cluster [blue]'{cluster}'[/]");

    public static void LogCommandCompleted(this IAnsiConsole console) =>
        console.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed[/]");
}
