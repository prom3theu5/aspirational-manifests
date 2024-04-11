namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceBindingsSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public const string BindingPlaceholder = "bindings";
    private int _servicePort = 10000;

    public bool CanSubstitute(KeyValuePair<string, string> placeholder, bool ignorePlaceholder = false)
    {
        if (ignorePlaceholder)
        {
            return true;
        }

        return placeholder.Value.Contains($".{BindingPlaceholder}.", StringComparison.OrdinalIgnoreCase) &&
               !placeholder.Key.Contains(ResourceConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder,
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
        var resourceName = parts[0];

        if (parts.Length < 4)
        {
            throw new InvalidOperationException($"Binding placeholder {placeholder} is not in the expected format.");
        }

        var bindingName = parts[2];
        var bindingProperty = parts[3];
        var requiredResource = resources[resourceName];

        // Ensure the resource has bindings
        if (requiredResource is not IResourceWithBinding resourceWithBinding || !resourceWithBinding.Bindings.TryGetValue(bindingName, out var binding))
        {
            throw new InvalidOperationException($"Binding {bindingName} not found for resource {resourceName}.");
        }

        var newValue = bindingProperty switch
        {
            "host" => resourceName,  // return the name of the resource for 'host'
            "port" => binding.TargetPort.ToString(),
            "url" => HandleUrlBinding(resourceName, bindingName, binding),
            _ => throw new InvalidOperationException($"Unknown property {bindingProperty} in placeholder {placeholder}.")
        };

        resourceWithEnv.Env[placeholder.Key] = newValue;
    }

    public void Reset() => _servicePort = 10000;

    private string HandleUrlBinding(string resourceName, string bindingName, Binding binding) =>
        bindingName switch
        {
            "http" => $"http://{resourceName}:{binding.TargetPort}",
            "https" =>  string.Empty, // For now - disable https, only http is supported until we have a way to generate dev certs and inject into container for startup.
            _ => HandleCustomServicePortBinding(resourceName, binding),
        };

    private string HandleCustomServicePortBinding(string resourceName, Binding binding)
    {
        if (binding.TargetPort == 0)
        {
            binding.TargetPort = _servicePort;
            _servicePort++;
        }

        var prefix = HandleServiceBindingPrefix(binding);

        return $"{prefix}{resourceName}:{binding.TargetPort}";
    }

    private static string HandleServiceBindingPrefix(Binding binding) =>
        binding.Protocol switch
        {
            "http" => "http://",
            "https" => "https://",
            _ => string.Empty,
        };
}
