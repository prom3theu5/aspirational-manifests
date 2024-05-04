namespace Aspirate.Commands.Actions.Secrets;

public class SaveSecretsAction(
    IAnsiConsole console,
    ISecretService secretService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Populating Secrets File[/]");

        secretService.SaveSecrets(new SecretManagementOptions
        {
            State = CurrentState,
            NonInteractive = CurrentState.NonInteractive,
            DisableSecrets = CurrentState.DisableSecrets,
            SecretPassword = CurrentState.SecretPassword
        });

        return Task.FromResult(true);
    }
}
