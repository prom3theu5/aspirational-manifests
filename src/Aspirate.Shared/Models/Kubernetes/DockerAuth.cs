namespace Aspirate.Shared.Models.Kubernetes;

public class DockerAuth
{
    [JsonPropertyName("auth")]
    public string? Auth { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
