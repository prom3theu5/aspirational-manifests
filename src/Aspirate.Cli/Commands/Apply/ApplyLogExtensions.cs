namespace Aspirate.Cli.Commands.Apply;

public static class ApplyLogExtensions
{
    public static void LogApplyCommandCompleted(this IAnsiConsole console, string cluster) =>
        console.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done:[/] Deployments successfully applied to cluster [blue]'{cluster}'[/]");

    public static void LogCommandCompleted(this IAnsiConsole console) =>
        console.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed - Happy Deployment {EmojiLiterals.Smiley}[/]");
}
