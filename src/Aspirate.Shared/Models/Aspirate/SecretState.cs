namespace Aspirate.Shared.Models.Aspirate;

public sealed class SecretState
{
    [JsonPropertyName("salt")]
    [RestorableStateProperty]
    public string? Salt { get; set; }

    [JsonPropertyName("hash")]
    [RestorableStateProperty]
    public string? Hash { get; set; }

    [JsonPropertyName("secrets")]
    [RestorableStateProperty]
    public Dictionary<string, Dictionary<string, string>> Secrets { get; set; } = [];

    [JsonPropertyName("secretsVersion")]
    [RestorableStateProperty]
    public int? Version { get; set; } = 0;
}
