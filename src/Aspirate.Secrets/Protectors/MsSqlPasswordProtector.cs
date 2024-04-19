namespace Aspirate.Secrets.Protectors;

public class MsSqlPasswordProtector(ISecretProvider secretProvider, IAnsiConsole console) : BaseProtector(secretProvider, console)
{
    public override bool HasSecrets(KeyValuePair<string, Resource> component)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return false;
        }

        return componentWithEnv.Env?.Any(x => x.Key.Equals(ProtectorType.MsSqlPassword.Value, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    public override void ProtectSecrets(KeyValuePair<string, Resource> component, bool nonInteractive)
    {
        if (component.Value is not IResourceWithEnvironmentalVariables componentWithEnv)
        {
            return;
        }

        var msSqlPasswordInput = componentWithEnv.Env.FirstOrDefault(x => x.Key.Equals(ProtectorType.MsSqlPassword.Value, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(msSqlPasswordInput.Key) && msSqlPasswordInput.Key.Equals(ProtectorType.MsSqlPassword.Value, StringComparison.OrdinalIgnoreCase))
        {
            UpsertSecret(component, msSqlPasswordInput, nonInteractive);
        }
    }
}
