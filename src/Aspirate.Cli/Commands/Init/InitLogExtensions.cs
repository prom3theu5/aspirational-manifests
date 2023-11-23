namespace Aspirate.Cli.Commands.Init;

public static class InitLogExtensions
{
    public static void LogCommandCompleted(this IAnsiConsole console) =>
        console.MarkupLine($"\r\n[bold] {EmojiLiterals.Rocket} Execution Completed[/]");
}
