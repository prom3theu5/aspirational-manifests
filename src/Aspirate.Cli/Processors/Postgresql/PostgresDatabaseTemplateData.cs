namespace Aspirate.Cli.Processors.Postgresql;

public sealed class PostgresDatabaseTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(name, env, manifests, isService);
