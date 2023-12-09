namespace Aspirate.Processors.SqlServer;

public sealed class SqlServerDatabaseTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(null, null, null, manifests, false);
