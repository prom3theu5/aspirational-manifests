namespace Aspirate.Secrets.Extensions;

public static class ProtectionStrategyExtensions
{
    public static bool CheckForProtectableSecrets(
        this IReadOnlyCollection<ISecretProtectionStrategy> protectors,
        IReadOnlyCollection<KeyValuePair<string, Resource>> components)
    {
        bool protectableSecrets = false;

        foreach (var component in components)
        {
            if (component.Value.Env is null)
            {
                continue;
            }

            if (protectors.Any(strategy => strategy.HasSecrets(component)))
            {
                protectableSecrets = true;
            }

            if (protectableSecrets)
            {
                break;
            }
        }

        return protectableSecrets;
    }
}
