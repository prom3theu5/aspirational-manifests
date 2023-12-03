namespace Aspirate.Secrets;

public abstract class BaseSecretState
{
    [JsonPropertyName("secrets")]
    public Dictionary<string, string> Secrets { get; set; } = [];

    [JsonPropertyName("version")]
    public int? Version { get; set; } = 0;
}
