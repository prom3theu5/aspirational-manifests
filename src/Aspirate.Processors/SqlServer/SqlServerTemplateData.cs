namespace Aspirate.Processors.SqlServer;

public sealed class SqlServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, null, manifests, false)
{
    public required string SaPassword { get; set; }
}
