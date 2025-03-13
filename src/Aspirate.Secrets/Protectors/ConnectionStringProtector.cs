namespace Aspirate.Secrets.Protectors;

public class ConnectionStringProtector(ISecretProvider secretProvider, IAnsiConsole console): BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return false;
        }

        return componentWithEnv.Env?.Any(x => x.Key.StartsWith(ProtectorType.ConnectionString.Value)) ?? false;
    }

    public override void ProtectSecrets(KeyValuePair<string, Resource> component, bool nonInteractive)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return;
        }

        var connectionStrings = componentWithEnv.Env?.Where(x => x.Key.StartsWith(ProtectorType.ConnectionString.Value, StringComparison.OrdinalIgnoreCase)).ToList();

        if (connectionStrings.Count == 0)
        {
            return;
        }

        foreach (var input in connectionStrings)
        {
            UpsertSecret(component, input, nonInteractive);
        }
    }
}
