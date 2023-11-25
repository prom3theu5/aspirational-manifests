using Command = System.CommandLine.Command;

namespace Aspirate.Cli.Commands;

public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : class, ICommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description) =>
        this.Handler = CommandHandler.Create<TOptions, IServiceProvider, CancellationToken>(HandleOptions);

    private static Task<int> HandleOptions(TOptions options, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Wire up DI...
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(serviceProvider);
        return handler.HandleAsync(options, cancellationToken);
    }
}
