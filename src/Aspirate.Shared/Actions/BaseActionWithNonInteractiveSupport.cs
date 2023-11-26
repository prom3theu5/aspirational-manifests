namespace Aspirate.Shared.Actions;

public abstract class BaseActionWithNonInteractiveSupport(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public abstract void ValidateNonInteractiveState();

    protected void NonInteractiveValidationFailed(string message)
    {
        Logger.MarkupLine($"\r\n[red](!)[/] {message}");
        throw new ActionCausesExitException(9999);
    }
}
