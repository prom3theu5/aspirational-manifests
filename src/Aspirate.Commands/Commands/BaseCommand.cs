namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public abstract class BaseCommand<TOptions, TOptionsHandler> : Command
    where TOptions : BaseCommandOptions
    where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
{
    protected abstract bool CommandUnlocksSecrets { get; }
    protected virtual bool CommandSkipsStateAndSecrets => false;
    protected virtual bool CommandAlwaysRequiresState => false;

    protected BaseCommand(string name, string description)
        : base(name, description)
    {
        AddOption(NonInteractiveOption.Instance);
        AddOption(DisableSecretsOption.Instance);
        AddOption(DisableStateOption.Instance);
        Handler = CommandHandler.Create<TOptions, IServiceCollection>(ConstructCommand);
    }

    private async Task<int> ConstructCommand(TOptions options, IServiceCollection services)
    {
        var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(services.BuildServiceProvider());

        if (CommandSkipsStateAndSecrets)
        {
            return await handler.HandleAsync(options);
        }

        var stateService = handler.Services.GetRequiredService<IStateService>();
        var secretService = handler.Services.GetRequiredService<ISecretService>();
        var versionCheckService = handler.Services.GetRequiredService<IVersionCheckService>();

        await versionCheckService.CheckVersion();

        var stateOptions = GetStateManagementOptions(options, handler, CommandAlwaysRequiresState);

        await stateService.RestoreState(stateOptions);

        handler.CurrentState.PopulateStateFromOptions(options);

        LoadSecrets(options, secretService, handler);

        var exitCode = await handler.HandleAsync(options);

        await stateService.SaveState(stateOptions);

        return exitCode;
    }

    private void LoadSecrets(TOptions options, ISecretService secretService, TOptionsHandler handler)
    {
        var outputFormat = !string.IsNullOrEmpty(handler.CurrentState.OutputFormat) ? OutputFormat.FromValue(handler.CurrentState.OutputFormat) : OutputFormat.Kustomize;

        if (options is IGenerateOptions generateOptions)
        {
            if (OutputFormat.TryFromValue(generateOptions.OutputFormat, out var format))
            {
                outputFormat = format;
            }
        }

        var isKubernetes = outputFormat.Equals(OutputFormat.Kustomize) || outputFormat.Equals(OutputFormat.Helm);
        var isCompose = outputFormat.Equals(OutputFormat.DockerCompose);

        if (isCompose)
        {
            handler.CurrentState.DisableSecrets = true;
        }

        var shouldDisableSecrets = handler.CurrentState.DisableSecrets ?? ((isKubernetes) ? options.DisableSecrets : true);

        handler.CurrentState.DisableSecrets = shouldDisableSecrets;

        secretService.LoadSecrets(new SecretManagementOptions
        {
            DisableSecrets = handler.CurrentState.DisableSecrets,
            NonInteractive = options.NonInteractive,
            SecretPassword = options.SecretPassword,
            CommandUnlocksSecrets = CommandUnlocksSecrets,
            State = handler.CurrentState,
        });
    }

    private static StateManagementOptions GetStateManagementOptions(TOptions options, TOptionsHandler handler, bool requiresState) =>
        new()
        {
            NonInteractive = options.NonInteractive,
            DisableState = options.DisableState,
            State = handler.CurrentState,
            RequiresState = requiresState,
        };
}
