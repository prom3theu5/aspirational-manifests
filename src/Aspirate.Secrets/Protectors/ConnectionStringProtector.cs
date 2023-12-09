using Aspirate.Secrets.Literals;

namespace Aspirate.Secrets.Protectors;

public class ConnectionStringProtector(ISecretProvider secretProvider, IAnsiConsole console): BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component) =>
        component.Value.Env?.Any(x => x.Key.StartsWith(ProtectableLiterals.ConnectionString)) ?? false;

    public override void ProtectSecrets(KeyValuePair<string, Resource> component)
    {
        var connectionStrings = component.Value.Env?.Where(x => x.Key.StartsWith(ProtectableLiterals.ConnectionString, StringComparison.OrdinalIgnoreCase)).ToList();

        if (connectionStrings.Count == 0)
        {
            return;
        }

        foreach (var input in connectionStrings)
        {
            UpsertSecret(component, input);
        }
    }
}
