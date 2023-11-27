namespace Aspirate.Cli.Processors.RabbitMQ;

public sealed class RabbitMqTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
