namespace Aspirate.Cli.Processors.Components.Postgresql;

public sealed class PostgresServerTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<int> containerPorts,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(name, env, containerPorts, manifests, isService);
