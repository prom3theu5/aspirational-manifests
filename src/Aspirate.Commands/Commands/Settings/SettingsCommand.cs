namespace Aspirate.Commands.Commands.Settings;

public sealed class SettingsCommand : GenericCommand
{
    public SettingsCommand() : base("settings", "Managed Aspir8 settings.")
    {
        AddCommand(new UpdateChecksCommand());
        AddCommand(new LogoCommand());
    }

    protected override Task<int> ExecuteCommand(IServiceCollection services)
    {
        var table = CreateHelpTable();

        table.AddRow("update-checks", "Enable / Disable Aspir8 version update checks.");
        table.AddRow("logo", "Show / Hide the Aspir8 logo.");

        AnsiConsole.Render(table);
        return Task.FromResult(0);
    }
}

internal sealed class LogoCommand : GenericCommand
{
    public LogoCommand() : base("logo", "Show / Hide the Aspir8 logo.")
    {
        AddCommand(new ShowLogoCommand());
        AddCommand(new HideLogoCommand());
    }

    protected override Task<int> ExecuteCommand(IServiceCollection services)
    {
        var table = CreateHelpTable();

        table.AddRow("show", "Show the Aspir8 logo.");
        table.AddRow("hide", "Hide the Aspir8 logo.");

        AnsiConsole.Render(table);
        return Task.FromResult(0);
    }
}

internal sealed class UpdateChecksCommand : GenericCommand
{
    public UpdateChecksCommand() : base("update-checks", "Manage Aspirate Version Checks.")
    {
        AddCommand(new EnableUpdateChecksCommand());
        AddCommand(new DisableUpdateChecksCommand());
    }

    protected override Task<int> ExecuteCommand(IServiceCollection services)
    {
        var table = CreateHelpTable();

        table.AddRow("enable", "Enables Aspir8 version checks.");
        table.AddRow("disable", "Disables Aspir8 version checks.");

        AnsiConsole.Render(table);
        return Task.FromResult(0);
    }
}

internal sealed class EnableUpdateChecksCommand() : GenericCommand("enable", "Enables Aspir8 version checks.")
{
    protected override async Task<int> ExecuteCommand(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var versionCheckService = serviceProvider.GetRequiredService<IVersionCheckService>();
        await versionCheckService.SetUpdateChecks(true);
        return 0;
    }
}

internal sealed class DisableUpdateChecksCommand() : GenericCommand("disable", "Disables Aspir8 version checks.")
{
    protected override async Task<int> ExecuteCommand(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var versionCheckService = serviceProvider.GetRequiredService<IVersionCheckService>();
        await versionCheckService.SetUpdateChecks(false);
        return 0;
    }
}

internal sealed class ShowLogoCommand() : GenericCommand("show", "Show the aspirate Logo.")
{
    protected override Task<int> ExecuteCommand(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        var logger = serviceProvider.GetRequiredService<IAnsiConsole>();
        var appDataFolder = fileSystem.AspirateAppDataFolder();
        var logoFilePath = fileSystem.Path.Combine(appDataFolder, AspirateLiterals.LogoDisabledFile);

        if (fileSystem.File.Exists(logoFilePath))
        {
            fileSystem.File.Delete(logoFilePath);
            logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] The Aspir8 logo will now be [blue]shown[/].");

        }

        return Task.FromResult(0);
    }
}

internal sealed class HideLogoCommand() : GenericCommand("hide", "Hide the Aspir8 logo.")
{
    protected override async Task<int> ExecuteCommand(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        var logger = serviceProvider.GetRequiredService<IAnsiConsole>();
        var appDataFolder = fileSystem.AspirateAppDataFolder();
        var logoFilePath = fileSystem.Path.Combine(appDataFolder, AspirateLiterals.LogoDisabledFile);

        await fileSystem.File.WriteAllTextAsync(logoFilePath, "1");
        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done:[/] The Aspir8 logo has been [blue]hidden[/].");

        return 0;
    }
}
