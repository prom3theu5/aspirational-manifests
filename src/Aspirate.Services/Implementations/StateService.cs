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

        if (options.DisableState == true)
        {
            logger.MarkupLine("State has been [red]disabled[/] for this run.");
            return;
        }

        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);

        if (options.NonInteractive == true)
        {
            await RestoreState(options, stateFile, true);
            logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");
            return;
        }

        if (!fs.File.Exists(stateFile))
        {
            return;
        }

        logger.MarkupLine($"[bold]Loading state from [blue]{stateFile}[/].[/]");

        var shouldUseAllPreviousState = logger.Confirm("Would you like to use all previous state values, and [blue]skip[/] re-prompting where possible ?");

        if (shouldUseAllPreviousState)
        {
            logger.MarkupLine("[bold]Using all previous state values, and skipping re-prompting.[/]");
            await RestoreState(options, stateFile, true);
            options.State.UseAllPreviousStateValues = true;
        }
        else
        {
            logger.MarkupLine("[bold]Re-prompting for all state values not specified on command line.[/]");
            await RestoreState(options, stateFile, false);
            options.State.UseAllPreviousStateValues = false;
        }

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");
    }

    private async Task RestoreState(StateManagementOptions options, string stateFile, bool shouldUseAllPreviousStateValues)
    {
        var stateAsJson = await fs.File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<AspirateState>(stateAsJson, _jsonSerializerOptions);

        options.State.ReplaceCurrentStateWithPreviousState(previousState, shouldUseAllPreviousStateValues);
    }
}
