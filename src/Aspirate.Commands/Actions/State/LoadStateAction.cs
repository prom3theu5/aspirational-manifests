namespace Aspirate.Commands.Actions.State;

public class LoadStateAction(IServiceProvider serviceProvider, IFileSystem fileSystem)
    : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Checking for existing state[/]");

        var stateFile = fileSystem.Path.Combine(fileSystem.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);

        if (!fileSystem.File.Exists(stateFile))
        {
            return true;
        }

        var shouldLoadState = Logger.Confirm("A previous state file was found. Would you like to load it?");

        if (!shouldLoadState)
        {
            Logger.MarkupLine("[bold]Skipping loading state.[/]");
            return true;
        }

        Logger.MarkupLine($"[bold]Loading state from [blue]{stateFile}[/].[/]");

        var stateAsJson = await fileSystem.File.ReadAllTextAsync(stateFile);
        var previousState = JsonSerializer.Deserialize<AspirateState>(stateAsJson);

        CurrentState.ReplaceCurrentStateWithPreviousState(previousState);

        var shouldUseAllPreviousState = Logger.Confirm("Would you like to use all previous state values, and skip re-prompting (Except for secrets) ?");

        if (shouldUseAllPreviousState)
        {
            Logger.MarkupLine("[bold]Using all previous state values, and skipping re-prompting.[/]");
            CurrentState.UseAllPreviousStateValues = true;
        }

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State loaded successfully from [blue]{stateFile}[/]");

        return true;
    }
}
