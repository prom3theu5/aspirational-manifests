namespace Aspirate.Shared.Models.Aspirate;

public sealed class LastVersionChecked
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("lastChecked")]
    public DateTime LastChecked { get; set; }
}
