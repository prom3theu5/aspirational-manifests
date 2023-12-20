namespace Aspirate.Secrets.Protectors;

public class PostgresPasswordProtector(ISecretProvider secretProvider, IAnsiConsole console): BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component) =>
        component.Value.Env?.Any(x => x.Key.Equals(ProtectableLiterals.PostgresPassword, StringComparison.OrdinalIgnoreCase)) ?? false;

    public override void ProtectSecrets(KeyValuePair<string, Resource> component)
    {
        var postgresPasswordInput = component.Value.Env?.FirstOrDefault(x => x.Key.Equals(ProtectableLiterals.PostgresPassword, StringComparison.OrdinalIgnoreCase));

        if (postgresPasswordInput is not { Key: ProtectableLiterals.PostgresPassword })
        {
            return;
        }

        UpsertSecret(component, postgresPasswordInput.GetValueOrDefault());
    }
}
