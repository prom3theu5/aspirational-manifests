namespace Aspirate.Commands.Actions;

public abstract class BaseActionWithNonInteractiveValidation(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public abstract void ValidateNonInteractiveState();
}
