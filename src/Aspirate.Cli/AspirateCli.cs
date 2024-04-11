namespace Aspirate.Cli;

internal class AspirateCli : RootCommand
{
    internal static void WelcomeMessage()
    {
        var skipLogo = Environment.GetEnvironmentVariable("ASPIRATE_NO_LOGO");

        if (!string.IsNullOrEmpty(skipLogo))
        {
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("Aspir8").Color(Color.HotPink));
        AnsiConsole.MarkupLine("[bold lime]Handle deployments of a .NET Aspire AppHost[/]");
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
