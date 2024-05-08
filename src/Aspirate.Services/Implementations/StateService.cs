namespace Aspirate.Services.Implementations;

public class StateService(IFileSystem fs, IAnsiConsole logger, ISecretProvider secretProvider) : IStateService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        IncludeFields = true,
        Converters =
        {
            new SmartEnumNameConverter<ExistingSecretsType, string>()
        }
    };

    public async Task SaveState(StateManagementOptions options)
    {
        if (options.DisableState == true)
        {
            return;
        }

        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);
        var stateAsJson = JsonSerializer.Serialize(options.State, _jsonSerializerOptions);

        await fs.File.WriteAllTextAsync(stateFile, stateAsJson);
    }

    public async Task RestoreState(StateManagementOptions options)
    {
        logger.WriteRuler("[purple]Handling Aspirate State[/]");

        if (ShouldCancelAsStateFileDoesNotExist(out var stateFile))
        {
            return;
        }

        if (await IsNonInteractiveMode(options, stateFile))
        {
            return;
        }

        if (options.DisableState == true)
        {
            await OnlyRestoreSecrets(options, stateFile);
            return;
        }

        if (await ShouldUseAllPreviousState(options, stateFile))
        {
            return;
        }

        await OnlyRestoreSecrets(options, stateFile);
    }

    private Task<bool> IsNonInteractiveMode(StateManagementOptions options, string stateFile) =>
        options switch
        {
            { NonInteractive: true, DisableState: true } => OnlyRestoreSecrets(options, stateFile),
            { NonInteractive: true, DisableState: false or null } => RestoreAllState(options, stateFile),
            _ => Task.FromResult(false)
        };

    private bool ShouldCancelAsStateFileDoesNotExist(out string stateFile)
    {
        stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);

        return !fs.File.Exists(stateFile);
    }

    private async Task<bool> ShouldUseAllPreviousState(StateManagementOptions options, string stateFile)
    {
        logger.MarkupLine($"[bold]Loading state from [blue]{stateFile}[/].[/]");

        var shouldUseAllPreviousState = logger.Confirm("Would you like to use all previous state values, and [blue]skip[/] re-prompting where possible ?");

        if (!shouldUseAllPreviousState)
        {
            return false;
        }

        await RestoreAllState(options, stateFile);
        return true;
    }

    private async Task<bool> RestoreAllState(StateManagementOptions options, string stateFile)
    {
        await RestoreState(options, stateFile, true);
        LogAllStateReloaded(stateFile);
        return true;
    }

    private async Task<bool> OnlyRestoreSecrets(StateManagementOptions options, string stateFile)
    {
        await RestoreState(options, stateFile, false);
        LogDisabledStateMessage();
        return true;
    }

    private async Task RestoreState(StateManagementOptions options, string stateFile, bool shouldUseAllPreviousStateValues)
    {
        var stateAsJson = await fs.File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<AspirateState>(stateAsJson, _jsonSerializerOptions);
        options.State.ReplaceCurrentStateWithPreviousState(previousState, shouldUseAllPreviousStateValues);
        options.State.UseAllPreviousStateValues = shouldUseAllPreviousStateValues;
    }

    private void LogDisabledStateMessage() =>
        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State has been disabled for this run. Only secrets will be populated.");

    private void LogAllStateReloaded(string stateFile) =>
        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]. Will run without re-prompting for values.");
}
