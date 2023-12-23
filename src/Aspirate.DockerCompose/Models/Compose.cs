namespace Aspirate.DockerCompose.Models;

[Serializable]
public class Compose
{
    [YamlMember(Alias = "version")]
    public string? Version { get; set; }

    [YamlMember(Alias = "services")]
    public IDictionary<string, Service>? Services { get; set; }

    [YamlMember(Alias = "networks")]
    public IDictionary<string, Network>? Networks { get; set; }

    [YamlMember(Alias = "secrets")]
    public IDictionary<string, Secret>? Secrets { get; set; }

    [YamlMember(Alias = "volumes")]
    public IDictionary<string, Volume>? Volumes { get; set; }
}
