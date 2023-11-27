namespace Aspirate.Cli.Processors.Postgresql;

public sealed class PostgresServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
