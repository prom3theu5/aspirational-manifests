namespace Aspirate.Shared.Models.Kubernetes;

public class DockerConfigJson
{
    [JsonPropertyName("auths")]
    public Dictionary<string, DockerAuth>? Auths { get; set; }
}
