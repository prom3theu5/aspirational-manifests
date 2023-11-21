namespace Aspirate.Cli.Processors.Components.RabbitMQ;

public sealed class RabbitMqTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
