using Aspirate.Shared.Literals;

namespace Aspirate.Shared.Models.MsBuild;

[ExcludeFromCodeCoverage]
public sealed class MsBuildPublishingProperties : BaseMsBuildProperties
{
    [JsonPropertyName(MsBuildPropertiesLiterals.PublishSingleFileArgument)]
    public string? PublishSingleFile { get; set; }

    [JsonPropertyName(MsBuildPropertiesLiterals.PublishTrimmedArgument)]
    public string? PublishTrimmed { get; set; }
}
