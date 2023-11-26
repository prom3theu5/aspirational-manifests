namespace Aspirate.Shared.Commands;

public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : class, ICommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description) =>
        this.Handler = CommandHandler.Create<TOptions, IServiceProvider>(HandleOptions);

    private static Task<int> HandleOptions(TOptions options, IServiceProvider serviceProvider)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(serviceProvider);

        handler.CurrentState.PopulateStateFromOptions(options);

        return handler.HandleAsync(options);
    }
}
