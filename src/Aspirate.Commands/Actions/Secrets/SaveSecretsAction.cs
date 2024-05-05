namespace Aspirate.Commands.Actions.Secrets;

public class SaveSecretsAction(
    IAnsiConsole console,
    ISecretService secretService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Populating Secrets File[/]");

        var isKustomize = CurrentState.OutputFormat.Equals(OutputFormat.Kustomize.Value, StringComparison.OrdinalIgnoreCase);

        secretService.SaveSecrets(new SecretManagementOptions
        {
            State = CurrentState,
            NonInteractive = CurrentState.NonInteractive,
            DisableSecrets = isKustomize ? CurrentState.DisableSecrets : true,
            SecretPassword = CurrentState.SecretPassword,
        });

        return Task.FromResult(true);
    }
}
