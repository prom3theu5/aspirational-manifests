namespace Aspirate.Shared.Actions;

public abstract class BaseAction(IServiceProvider serviceProvider) : IAction
{
    protected IAnsiConsole Logger { get; } = serviceProvider.GetRequiredService<IAnsiConsole>();
    protected AspirateState CurrentState { get; } = serviceProvider.GetRequiredService<AspirateState>();
    protected IServiceProvider Services { get; } = serviceProvider;

    public abstract Task<bool> ExecuteAsync();
}
