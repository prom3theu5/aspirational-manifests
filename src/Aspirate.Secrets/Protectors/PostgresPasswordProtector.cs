namespace Aspirate.Secrets.Protectors;

public class PostgresPasswordProtector(ISecretProvider secretProvider, IAnsiConsole console) : BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component) =>
        component.Value.Env?.Any(x => x.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase)) ?? false;

    public override void ProtectSecrets(KeyValuePair<string, Resource> component)
    {
        if (component.Value.Env is null)
        {
            return;
        }

        var postgresPasswordInput = component.Value.Env.FirstOrDefault(x => x.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(postgresPasswordInput.Key) && postgresPasswordInput.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase))
        {
            UpsertSecret(component, postgresPasswordInput);
        }
    }
}
