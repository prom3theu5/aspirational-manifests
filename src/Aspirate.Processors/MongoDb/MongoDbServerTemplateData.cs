namespace Aspirate.Processors.MongoDb;

public sealed class MongoDbServerTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, null, manifests, false);
