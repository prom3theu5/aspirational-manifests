namespace Aspirate.Shared.Extensions;

public static class AnsiConsoleExtensions
{
    public static void WriteRuler(this IAnsiConsole console, string message, Justify justification = Justify.Left)
    {
        var rule = new Rule(message) { Justification = justification };
        console.WriteLine();
        console.Write(rule);
    }

    public static void ValidationFailed(this IAnsiConsole console, string message)
    {
        console.MarkupLine($"[red](!)[/] {message}");
        ActionCausesExitException.ExitNow();
    }
}
