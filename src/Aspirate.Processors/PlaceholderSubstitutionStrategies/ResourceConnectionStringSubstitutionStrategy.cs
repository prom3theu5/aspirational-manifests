namespace Aspirate.Processors.PlaceholderSubstitutionStrategies;

public partial class ResourceConnectionStringSubstitutionStrategy : IPlaceholderSubstitutionStrategy
{
    [GeneratedRegex(@"\{([\w\.-]+)\}")]
    private static partial Regex ConnectionStringRegex();

    public const string ConnectionStringPlaceholder = "connectionString";
    public const string InputsPlaceholder = "value";

    public bool CanSubstitute(KeyValuePair<string, string> placeholder, bool ignorePlaceholder = false)
    {
        if (ignorePlaceholder)
        {
            return true;
        }

        return placeholder.Key.Contains(ConnectionStringPlaceholder, StringComparison.OrdinalIgnoreCase);
    }

    public void Substitute(KeyValuePair<string, string> placeholder, Dictionary<string, Resource> resources, Resource resource)
    {
        if (resource is ValueResource)
        {
            return;
        }

        if (resource is not IResourceWithConnectionString resourceWithConnectionString)
        {
            return;
        }

        if (!string.IsNullOrEmpty(resourceWithConnectionString.ConnectionString))
        {
            resourceWithConnectionString.ConnectionString = ReplaceConnectionStringPlaceholders(resources, resourceWithConnectionString.ConnectionString);
        }
    }

    public void Reset() { }

    private static string ReplaceConnectionStringPlaceholders(IReadOnlyDictionary<string, Resource> resources, string input) =>
        ConnectionStringRegex()
            .Replace(input, match =>
            {
                string[] pathParts = match.Groups[1].Value.Split('.');
                if (!resources.TryGetValue(pathParts[0], out var resource))
                {
                    throw new ArgumentException($"Resource {pathParts[0]} not found.");
                }

                return pathParts[1] switch
                {
                    ResourceBindingsSubstitutionStrategy.BindingPlaceholder when pathParts[3] == "host"
                        => pathParts[0],

                    ResourceBindingsSubstitutionStrategy.BindingPlaceholder when pathParts[3] == "port" && resource is ContainerResource targetContainer && targetContainer.Bindings != null && targetContainer.Bindings.TryGetValue(pathParts[2], out var binding)
                        => binding.TargetPort.ToString(),

                    InputsPlaceholder when resource is ParameterResource parameter
                        => parameter.Value ?? string.Empty,

                    _ => throw new ArgumentException($"Unknown dictionary in placeholder {match.Value}."),
                };
            });
}
