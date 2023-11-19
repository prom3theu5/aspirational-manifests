namespace Aspirational.Manifests.ManifestHandlers.Components.Postgresql;

public sealed class PostgresDatabaseTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<int> containerPorts,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(name, env, containerPorts, manifests, isService);
