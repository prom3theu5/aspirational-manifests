namespace Aspirate.Cli;

internal class AspirateCli : RootCommand
{
    internal static void WelcomeMessage()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("Aspir8").Color(Color.HotPink));
        AnsiConsole.MarkupLine("[bold lime]Automate deployment of a .NET Aspire AppHost to a Kubernetes Cluster[/]");
        AnsiConsole.WriteLine();
    }

    public AspirateCli()
    {
        AddCommand(new InitCommand());
        AddCommand(new GenerateCommand());
        AddCommand(new BuildCommand());
        AddCommand(new ApplyCommand());
        AddCommand(new DestroyCommand());
    }
}
