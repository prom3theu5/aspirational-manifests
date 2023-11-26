namespace Aspirate.Shared.Actions;

public abstract class BaseActionWithNonInteractiveSupport(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    protected abstract void ValidateNonInteractiveState();
}
