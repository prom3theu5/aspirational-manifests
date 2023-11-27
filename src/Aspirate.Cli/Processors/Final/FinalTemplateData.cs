namespace Aspirate.Cli.Processors.Final;

public class FinalTemplateData(IReadOnlyCollection<string> manifests) : BaseTemplateData(null, null, manifests, false);
