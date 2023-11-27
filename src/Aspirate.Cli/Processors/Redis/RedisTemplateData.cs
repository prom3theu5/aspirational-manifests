namespace Aspirate.Cli.Processors.Redis;

public sealed class RedisTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
