namespace Aspirate.Cli.Processors.Components.Postgresql;

public sealed class PostgresServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
