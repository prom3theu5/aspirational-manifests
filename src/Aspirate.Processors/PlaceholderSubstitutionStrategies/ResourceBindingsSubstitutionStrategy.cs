namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceBindingsSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public const string BindingPlaceholder = "bindings";

    public bool CanSubstitute(KeyValuePair<string, string> placeholder) =>
        placeholder.Value.Contains($".{BindingPlaceholder}.", StringComparison.OrdinalIgnoreCase) &&
        !placeholder.Value.Contains(ResourceInputsSubstitutionStrategy.InputsPlaceholder, StringComparison.OrdinalIgnoreCase) &&
        !placeholder.Value.Contains(ResourceGenericConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder, StringComparison.OrdinalIgnoreCase);

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
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
            "port" => binding.ContainerPort.ToString(),
            "url" => parts[2] == "http" ? $"http://{resourceName}:8080" : $"https://{resourceName}:8443",
            _ => throw new InvalidOperationException($"Unknown property {bindingProperty} in placeholder {placeholder}.")
        };

        resource.Env[placeholder.Key] = newValue;
    }
}
