namespace Aspirate.Processors.Postgresql;

public sealed class PostgresServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, null, manifests, false);
