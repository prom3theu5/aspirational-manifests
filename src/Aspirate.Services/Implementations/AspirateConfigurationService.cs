namespace Aspirate.Services.Implementations;

public class AspirateConfigurationService(IAnsiConsole console, IFileSystem fileSystem) : IAspirateConfigurationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public void HandleExistingConfiguration(string appHostPath, bool nonInteractive = false)
    {
        var configurationPath = fileSystem.NormalizePath(appHostPath);
        var configurationFile = fileSystem.Path.Combine(configurationPath, AspirateSettings.FileName);

        if (!fileSystem.File.Exists(configurationFile))
        {
            return;
        }

        LogExistingConfigurationFound();

        if (!nonInteractive)
        {
            var shouldDelete = console.Confirm("Would you like to overwrite the existing configuration?");

            if (!shouldDelete)
            {
                LogExistingNotDeleted();
                throw new ActionCausesExitException(1);
            }
        }

        if (nonInteractive)
        {
            LogForceRemovalInNonInteractiveMode();
        }

        File.Delete(configurationFile);
        LogExistingConfigurationDeleted();
    }

    public AspirateSettings? LoadConfigurationFile(string appHostPath)
    {
        var configurationPath = fileSystem.NormalizePath(appHostPath);
        var configurationFile = fileSystem.Path.Combine(configurationPath, AspirateSettings.FileName);

        if (!fileSystem.File.Exists(configurationFile))
        {
            return null;
        }

        var configurationJson = fileSystem.File.ReadAllText(configurationFile);

        var aspirateSettings = JsonSerializer.Deserialize<AspirateSettings>(configurationJson, _jsonSerializerOptions);

        return aspirateSettings;
    }

    public void SaveConfigurationFile(AspirateSettings settings, string appHostPath)
    {
        var configurationPath = fileSystem.NormalizePath(appHostPath);
        var configurationFile = fileSystem.Path.Combine(configurationPath, AspirateSettings.FileName);

        if (fileSystem.File.Exists(configurationFile))
        {
            HandleExistingConfiguration(appHostPath);
        }

        var configurationJson = JsonSerializer.Serialize(settings, _jsonSerializerOptions);

        fileSystem.File.WriteAllText(configurationFile, configurationJson);

        LogConfigurationSaved(configurationFile);
    }

    private void LogExistingConfigurationFound() =>
        console.MarkupLine($"\r\n[bold yellow] {EmojiLiterals.Warning} Existing configuration found.[/]");

    private void LogExistingConfigurationDeleted() =>
        console.MarkupLine($"\r\n[bold green] {EmojiLiterals.Warning} Existing configuration has been [red]deleted[/].[/]");

    private void LogForceRemovalInNonInteractiveMode() =>
        console.MarkupLine($"\r\n[bold green] {EmojiLiterals.Warning} Force Removing as Currently in [blue]Non Interactive Mode[/].[/]");

    private void LogExistingNotDeleted() =>
        console.MarkupLine($"\r\n[bold red] {EmojiLiterals.Warning} Existing configuration has not been removed - aspirate will now terminate.[/]");

    private void LogConfigurationSaved(string path) =>
        console.MarkupLine($"\r\n[bold green]({EmojiLiterals.CheckMark}) Done:[/] Configuration for aspirate has been bootstrapped successfully at [blue]'{path}'.[/]");
}
