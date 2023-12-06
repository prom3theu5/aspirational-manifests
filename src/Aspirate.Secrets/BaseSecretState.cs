namespace Aspirate.Secrets;

public abstract class BaseSecretState
{
    [JsonPropertyName("secrets")]
    public Dictionary<string, string> Secrets { get; set; } = [];

    [JsonPropertyName("secretsVersion")]
    public int? Version { get; set; } = 0;
}
