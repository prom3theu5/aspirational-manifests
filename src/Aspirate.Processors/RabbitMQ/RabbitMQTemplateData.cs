namespace Aspirate.Processors.RabbitMQ;

public sealed class RabbitMqTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, null, manifests, false);
