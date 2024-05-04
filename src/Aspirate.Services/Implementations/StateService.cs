namespace Aspirate.Services.Implementations;

public class StateService(IFileSystem fs, IAnsiConsole logger, ISecretProvider secretProvider) : IStateService
{
    public async Task SaveState(StateManagementOptions options)
    {
        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);
        var stateAsJson = JsonSerializer.Serialize(options.State);

        await fs.File.WriteAllTextAsync(stateFile, stateAsJson);
    }

    public async Task RestoreState(StateManagementOptions options)
    {
        if (options.DisableState)
        {
            return;
        }

        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);

        if (options.NonInteractive)
        {
            await RestoreState(options, stateFile);
            logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");
            return;
        }

        if (!fs.File.Exists(stateFile))
        {
            return;
        }

        var shouldLoadState = logger.Confirm("A previous state file was found. Would you like to load it?");

        if (!shouldLoadState)
        {
            logger.MarkupLine("[bold]Skipping loading state.[/]");
            return;
        }

        logger.MarkupLine($"[bold]Loading state from [blue]{stateFile}[/].[/]");

        await RestoreState(options, stateFile);

        var shouldUseAllPreviousState = logger.Confirm("Would you like to use all previous state values, and skip re-prompting (Except for secrets) ?");

        if (shouldUseAllPreviousState)
        {
            logger.MarkupLine("[bold]Using all previous state values, and skipping re-prompting.[/]");
            options.State.UseAllPreviousStateValues = true;
        }

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");
    }

    private async Task RestoreState(StateManagementOptions options, string stateFile)
    {
        var stateAsJson = await fs.File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<AspirateState>(stateAsJson);

        options.State.ReplaceCurrentStateWithPreviousState(previousState);
    }
}
