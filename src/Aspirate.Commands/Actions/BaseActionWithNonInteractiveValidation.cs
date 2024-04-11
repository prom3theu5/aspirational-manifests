namespace Aspirate.Commands.Actions;

public abstract class BaseActionWithNonInteractiveValidation(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public abstract void ValidateNonInteractiveState();

    protected void NonInteractiveValidationFailed(string message)
    {
        Logger.MarkupLine($"[red](!)[/] {message}");
        ActionCausesExitException.ExitNow();
    }
}
