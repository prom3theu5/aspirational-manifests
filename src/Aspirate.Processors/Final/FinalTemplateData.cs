namespace Aspirate.Processors.Final;

public class FinalTemplateData(IReadOnlyCollection<string> manifests, string? ns)
    : BaseTemplateData(null, null, null, manifests, false)
{
    public string? Namespace { get; set; } = ns;
    public bool WithNamespace => !string.IsNullOrWhiteSpace(Namespace);
}
