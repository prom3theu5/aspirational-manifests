namespace Aspirate.CommandSupport.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : class, ICommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
        AddOption(NonInteractive);
    }

    private static Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        handler.CurrentState.PopulateStateFromOptions(options);

        return handler.HandleAsync(options);
    }

    private static Option<bool> NonInteractive => new(new[] { "--non-interactive" })
    {
        Description = "Disables interactive mode for the command",
        Arity = ArgumentArity.ZeroOrOne,
        IsRequired = false,
    };
}
