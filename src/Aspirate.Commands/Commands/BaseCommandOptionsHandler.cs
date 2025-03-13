namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommandOptionsHandler<TOptions> : ICommandOptionsHandler<TOptions> where TOptions : class, ICommandOptions
{
    protected BaseCommandOptionsHandler(IServiceProvider serviceProvider)
    {
        Services = serviceProvider;
        CurrentState = Services.GetRequiredService<AspirateState>();
        ActionExecutor = ActionExecutor.CreateInstance(serviceProvider);
    }

    public AspirateState CurrentState { get; set; }
    public IServiceProvider Services { get; }
    protected ActionExecutor ActionExecutor { get; set; }
    public abstract Task<int> HandleAsync(TOptions options);
}
