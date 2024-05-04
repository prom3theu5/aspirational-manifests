namespace Aspirate.Services.Implementations;

public class StateService(IFileSystem fs, IAnsiConsole logger) : IStateService
{
    public async Task SaveState(AspirateState state)
    {
        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);
        var stateAsJson = JsonSerializer.Serialize(state);

        await fs.File.WriteAllTextAsync(stateFile, stateAsJson);
    }

    public async Task RestoreState(AspirateState state)
    {
        var stateFile = fs.Path.Combine(fs.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);

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

        var stateAsJson = await fs.File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<AspirateState>(stateAsJson);

        state.ReplaceCurrentStateWithPreviousState(previousState);

        var shouldUseAllPreviousState = logger.Confirm("Would you like to use all previous state values, and skip re-prompting (Except for secrets) ?");

        if (shouldUseAllPreviousState)
        {
            logger.MarkupLine("[bold]Using all previous state values, and skipping re-prompting.[/]");
            state.UseAllPreviousStateValues = true;
        }

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");
    }
}
