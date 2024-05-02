using Aspirate.Shared.Interfaces.Secrets;

namespace Aspirate.Secrets.Protectors;

public class PostgresPasswordProtector(ISecretProvider secretProvider, IAnsiConsole console) : BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return false;
        }

        return componentWithEnv.Env?.Any(x => x.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    public override void ProtectSecrets(KeyValuePair<string, Resource> component, bool nonInteractive)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return;
        }

        var postgresPasswordInput = componentWithEnv.Env.FirstOrDefault(x => x.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(postgresPasswordInput.Key) && postgresPasswordInput.Key.Equals(ProtectorType.PostgresPassword.Value, StringComparison.OrdinalIgnoreCase))
        {
            UpsertSecret(component, postgresPasswordInput, nonInteractive);
        }
    }
}
