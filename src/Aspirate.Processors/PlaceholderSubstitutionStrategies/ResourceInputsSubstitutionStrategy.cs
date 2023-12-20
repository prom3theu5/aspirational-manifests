namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public sealed class ResourceInputsSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    public const string InputsPlaceholder = "inputs";

    public bool CanSubstitute(KeyValuePair<string, string> placeholder) =>
        placeholder.Value.Contains($".{InputsPlaceholder}.", StringComparison.OrdinalIgnoreCase) &&
        !placeholder.Value.Contains(ResourceBindingsSubstitutionStrategy.BindingPlaceholder, StringComparison.OrdinalIgnoreCase) &&
        !placeholder.Value.Contains(ResourceGenericConnectionStringSubstitutionStrategy.ConnectionStringPlaceholder, StringComparison.OrdinalIgnoreCase);

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
        var cleanPlaceholder = placeholder.Value.Trim('{', '}');

        var parts = cleanPlaceholder.Split('.');
        if (parts.Length < 3)
        {
            throw new InvalidOperationException($"Input placeholder {placeholder} is not in the expected format.");
        }

        var resourceName = parts[0];
        var requiredResource = resources[resourceName];
        var inputName = parts[2];

        if (requiredResource is not IResourceWithInput  resourceWithInput || !resourceWithInput.Inputs.TryGetValue(inputName, out var input))
        {
            throw new InvalidOperationException($"Input {inputName} not found for resource {resourceName}.");
        }

        resource.Env[placeholder.Key] = input.Value ?? string.Empty;
    }
}
