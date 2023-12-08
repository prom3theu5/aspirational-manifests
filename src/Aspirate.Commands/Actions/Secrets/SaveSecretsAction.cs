namespace Aspirate.Commands.Actions.Secrets;

public class SaveSecretsAction(
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        return Task.FromResult(true);
    }
}
