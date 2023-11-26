namespace Aspirate.Shared.Models.MsBuild;

[ExcludeFromCodeCoverage]
public sealed class MsBuildProperties<T> where T : BaseMsBuildProperties
{
    [JsonPropertyName("Properties")]
    public T? Properties { get; set; }
}
