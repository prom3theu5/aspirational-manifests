namespace Aspirate.Processors.Postgresql;

public sealed class PostgresDatabaseTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(null, null, null, manifests, false);
