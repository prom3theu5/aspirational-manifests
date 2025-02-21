using System.CommandLine.Help;
using Aspirate.Commands.Options;

namespace Aspirate.Cli;

internal class AspirateCli : RootCommand
{
    internal static void WelcomeMessage()
    {
        if (ShouldSkipLogo())
        {
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new FigletText("Aspir8").Color(Color.HotPink));
        AnsiConsole.MarkupLine("[bold lime]Handle deployments of a .NET Aspire AppHost[/]");
        AnsiConsole.WriteLine();
    }

    internal static void UseDefaultMasking(HelpContext helpContext)
    {
        var secretBaseOptions = helpContext.Command.Options
            .OfType<IBaseOption>()
            .Where(x => x.IsSecret)
            .ToArray();

        foreach (var secretBaseOption in secretBaseOptions)
        {
            AddDefaultMaskingForOption(helpContext, secretBaseOption);
        }
    }

    private static void AddDefaultMaskingForOption(HelpContext helpContext, IBaseOption baseOption) =>
        helpContext.HelpBuilder.CustomizeSymbol(
            (Symbol)baseOption,
            defaultValue: _ => new MaskedValue(baseOption.GetOptionDefault()?.ToString()).ToString());

    private static bool ShouldSkipLogo()
    {
        var appDataFolder = GetAppDataFolder();
        var skipLogoFile = Path.Combine(appDataFolder, AspirateLiterals.LogoDisabledFile);
        var skipLogo = Environment.GetEnvironmentVariable("ASPIRATE_NO_LOGO");

        return !string.IsNullOrEmpty(skipLogo) || File.Exists(skipLogoFile);
    }

    private static string GetAppDataFolder()
    {
        var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AspirateLiterals.AppDataFolder);

        if (!Directory.Exists(appDataFolder))
        {
            Directory.CreateDirectory(appDataFolder);
        }

        return appDataFolder;
    }

    public AspirateCli()
    {
        AddCommand(new InitCommand());
        AddCommand(new RunCommand());
        AddCommand(new StopCommand());
        AddCommand(new GenerateCommand());
        AddCommand(new BuildCommand());
        AddCommand(new ApplyCommand());
        AddCommand(new DestroyCommand());
        AddCommand(new SettingsCommand());
    }
}
