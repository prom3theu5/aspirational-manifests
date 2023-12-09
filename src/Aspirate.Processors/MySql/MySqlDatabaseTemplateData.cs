namespace Aspirate.Processors.MySql;

public sealed class MySqlDatabaseTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(null, null, null, manifests, false);
