namespace Aspirate.Processors.MySql;

public sealed class MySqlServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, null, manifests, false)
{
    public required string RootPassword { get; set; }
}
