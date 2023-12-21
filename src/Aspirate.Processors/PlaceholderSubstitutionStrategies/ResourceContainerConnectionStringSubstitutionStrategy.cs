namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public partial class ResourceContainerConnectionStringSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    [GeneratedRegex(@"\{([\w\.]+)\}")]
    private static partial Regex ConnectionStringRegex();

    public bool CanSubstitute(KeyValuePair<string, string> placeholder) => true;

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
        if (resource is not ContainerResource container)
        {
            throw new ArgumentException($"Resource {resource.Name} is not a container.");
        }

        container.ConnectionString = ReplaceConnectionStringPlaceholders(resources, container.ConnectionString);
    }

    private static string ReplaceConnectionStringPlaceholders(IReadOnlyDictionary<string, Resource> resources, string input) =>
        ConnectionStringRegex()
            .Replace(input, match =>
        {
            string[] pathParts = match.Groups[1].Value.Split('.');
            if (!resources.TryGetValue(pathParts[0], out var resource) || resource is not ContainerResource targetContainer)
            {
                throw new ArgumentException($"Resource {pathParts[0]} not found or is not a container.");
            }

            return pathParts[1] switch
            {
                ResourceBindingsSubstitutionStrategy.BindingPlaceholder when pathParts[3] == "host"
                    => pathParts[0],

                ResourceBindingsSubstitutionStrategy.BindingPlaceholder when pathParts[3] == "port" && targetContainer.Bindings != null && targetContainer.Bindings.TryGetValue(pathParts[2], out var binding)
                    => binding.ContainerPort.ToString(),

                ResourceInputsSubstitutionStrategy.InputsPlaceholder when targetContainer.Inputs != null && targetContainer.Inputs.TryGetValue(pathParts[2], out var inputEntry)
                    => inputEntry.Value ?? string.Empty,

                _ => throw new ArgumentException($"Unknown dictionary in placeholder {match.Value}."),
            };
        });
}
