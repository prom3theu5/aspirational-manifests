namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        AddOption(NonInteractiveOption.Instance);
        AddOption(SecretProviderOption.Instance);
        AddOption(DisableSecretsOption.Instance);
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private static Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        services.RegisterAspirateSecretProvider(options.SecretProvider);

        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        handler.CurrentState.PopulateStateFromOptions(options);

        return handler.HandleAsync(options);
    }
}
