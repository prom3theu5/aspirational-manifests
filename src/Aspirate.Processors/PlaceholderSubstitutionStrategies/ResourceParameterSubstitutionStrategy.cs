namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceParameterSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public bool CanSubstitute(KeyValuePair<string, string> placeholder, bool ignorePlaceholder = false)
    {
        if (ignorePlaceholder)
        {
            return true;
        }

        return !placeholder.Value.Contains(ResourceBindingsSubstitutionStrategy.BindingPlaceholder,
            StringComparison.OrdinalIgnoreCase);
    }

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
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
            HandleParameterResources(placeholder, resources, resourceValue, resourceWithEnv);
        }
    }

    private static void HandleParameterResources(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource matchedResource, IResourceWithEnvironmentalVariables resourceWithEnv)
    {
        if (matchedResource is ParameterResource parameterResource)
        {
            resourceWithEnv.Env[placeholder.Key] = parameterResource.Value ?? string.Empty;
        }
    }

    public void Reset() { }
}
