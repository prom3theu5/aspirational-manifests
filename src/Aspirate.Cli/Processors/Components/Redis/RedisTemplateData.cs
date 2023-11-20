namespace Aspirate.Cli.Processors.Components.Redis;

public sealed class RedisTemplateData(IReadOnlyCollection<string> manifests)
    : BaseTemplateData(null, null, manifests, false);
