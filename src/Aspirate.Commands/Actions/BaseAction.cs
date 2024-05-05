namespace Aspirate.Commands.Actions;

public abstract class BaseAction(IServiceProvider serviceProvider) : IAction
{
    protected IAnsiConsole Logger { get; } = serviceProvider.GetRequiredService<IAnsiConsole>();
    protected AspirateState CurrentState { get; } = serviceProvider.GetRequiredService<AspirateState>();
    protected IServiceProvider Services { get; } = serviceProvider;

    public abstract Task<bool> ExecuteAsync();
    protected virtual bool PreviousStateWasRestored(bool withConfirmation = true)
    {
        if (CurrentState.NonInteractive)
        {
            return false;
        }

        if (!CurrentState.StateWasLoadedFromPrevious)
        {
            return false;
        }

        if (!withConfirmation)
        {
            return true;
        }

        if (CurrentState.UseAllPreviousStateValues)
        {
            return true;
        }

        var shouldSkip = Logger.Confirm("Would you like to skip this action, and use the previous state?");

        if (!shouldSkip)
        {
            return false;
        }

        OnPreviousStateWasRestored();
        return true;
    }

    protected virtual void OnPreviousStateWasRestored() => Logger.MarkupLine("[bold]Skipping as it was already loaded from previous state.[/]");
}
