namespace Aspirate.Secrets.Protectors;

public abstract class BaseProtector(ISecretProvider secretProvider, IAnsiConsole console) : ISecretProtectionStrategy
{
    public abstract bool HasSecrets(KeyValuePair<string, Resource> component);

    public abstract void ProtectSecrets(KeyValuePair<string, Resource> component, bool nonInteractive);

    protected void UpsertSecret(KeyValuePair<string, Resource> component, KeyValuePair<string, string> input, bool nonInteractive) =>
        secretProvider.AddSecret(component.Key, input.Key, input.Value);
}
