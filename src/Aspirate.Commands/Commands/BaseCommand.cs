namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        AddOption(NonInteractive);
        AddOption(SecretProvider);
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private static Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        services.RegisterAspirateSecretProvider(options.SecretProvider);

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

    private static Option<ProviderType> SecretProvider => new(new[] { "--secret-provider" })
    {
        Description = "Sets the secret provider. Default is 'Password'",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
        IsHidden = true,
    };
}
