namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceValueSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public bool CanSubstitute(KeyValuePair<string, string> placeholder, bool ignorePlaceholder = false)
    {
        if (ignorePlaceholder)
        {
            return true;
        }

        return
            !placeholder.Value.Contains(ResourceBindingsSubstitutionStrategy.BindingPlaceholder, StringComparison.OrdinalIgnoreCase);
    }

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
        if (resource is ValueResource valueResource)
        {
            if (placeholder.Value.Contains(ResourceConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder, StringComparison.OrdinalIgnoreCase))
            {
                HandleConnectionStringPlaceholder(placeholder, resources, valueResource);
            }

            return;
        }

        if (resource is not IResourceWithEnvironmentalVariables resourceWithEnv)
        {
            return;
        }

        var cleanPlaceholder = placeholder.Value.Trim('{', '}');
        var parts = cleanPlaceholder.Split('.');
        if (parts.Length != 2)
        {
            throw new InvalidOperationException($"Placeholder {placeholder} is not in the expected format.");
        }

        var parameterName = parts[0];

        if (resources.TryGetValue(parameterName, out var resourceValue))
        {
            if (resourceValue is ValueResource referenceValueResource)
            {
                resourceWithEnv.Env[placeholder.Key] = referenceValueResource.Values[parts[1]].ToString();
                return;
            }

            if (resourceValue is ParameterResource parameterResource)
            {
                resourceWithEnv.Env[placeholder.Key] = parameterResource.Value;
                return;
            }

            if (placeholder.Value.Contains(ResourceConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder, StringComparison.OrdinalIgnoreCase))
            {
                if (resourceValue is IResourceWithConnectionString resourceWithConnectionString)
                {
                    resourceWithEnv.Env[placeholder.Key] = resourceWithConnectionString.ConnectionString;
                    return;
                }
            }
            
            if (resourceValue is IResourceWithEnvironmentalVariables resourceWithEnvValue)
            {
                resourceWithEnv.Env[placeholder.Key] = resourceWithEnvValue.Env[parts[1]];
                return;
            }
        }
    }

    private static void HandleConnectionStringPlaceholder(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, ValueResource valueResource)
    {
        var semiColonParts = placeholder.Value.Split(';');
        if (semiColonParts.Length < 1)
        {
            throw new InvalidOperationException($"Placeholder {placeholder} is not in the expected format.");
        }

        var cleanPlaceholder = semiColonParts[0].Trim('{', '}');
        var dotParts = cleanPlaceholder.Split('.');
        if (dotParts.Length != 2)
        {
            throw new InvalidOperationException($"Placeholder {cleanPlaceholder} is not in the expected format.");
        }

        var parameterName = dotParts[0];

        if (resources.TryGetValue(parameterName, out var resourceValue))
        {
            if (resourceValue is not IResourceWithConnectionString resourceWithConnectionString)
            {
                return;
            }

            valueResource.Values[ResourceConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder] = placeholder.Value.Replace(semiColonParts[0], resourceWithConnectionString.ConnectionString);
        }
    }

    public void Reset() { }
}
