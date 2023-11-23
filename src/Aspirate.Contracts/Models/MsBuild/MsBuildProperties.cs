namespace Aspirate.Contracts.Models.MsBuild;

[ExcludeFromCodeCoverage]
public sealed class MsBuildProperties<T> where T : BaseMsBuildProperties
{
    [JsonPropertyName("Properties")]
    public T? Properties { get; set; }
}
