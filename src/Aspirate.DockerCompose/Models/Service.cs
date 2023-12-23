namespace Aspirate.DockerCompose.Models;

[Serializable]
public class Service : IObject
{
    [YamlIgnore]
    public string Name { get; set; } = null!;

    [YamlMember(Alias = "container_name")]
    public string? ContainerName { get; set; }

    [YamlMember(Alias = "image")]
    public string? Image { get; set; }

    [YamlMember(Alias = "privileged")]
    public bool? Privileged { get; set; }

    [YamlMember(Alias = "build")]
    public ServiceBuild? Build { get; set; }

    [YamlMember(Alias = "healthcheck")]
    public HealthCheck? HealthCheck { get; set; }

    [YamlMember(Alias = "hostname")]
    public string? Hostname { get; set; }

    [YamlMember(Alias = "networks")]
    public List<string>? Networks { get; set; }

    [YamlMember(Alias = "labels")]
    public IDictionary<string, string>? Labels { get; set; }

    [YamlMember(Alias = "environment")]
    public IDictionary<string, string?>? Environment { get; set; }

    [YamlMember(Alias = "volumes")]
    public List<string>? Volumes { get; set; }

    [YamlMember(Alias = "ports")]
    public List<Port>? Ports { get; set; }

    [YamlMember(Alias = "extra_hosts")]
    public List<string>? ExtraHosts { get; set; }

    [YamlMember(Alias = "command")]
    public List<string>? Commands { get; set; }

    [YamlMember(Alias = "restart")]
    public RestartMode? Restart { get; set; }

    [YamlMember(Alias = "expose")]
    public List<string>? Expose { get; set; }

    [YamlMember(Alias = "secrets")]
    public List<string>? Secrets { get; set; }

    [YamlMember(Alias = "depends_on")]
    public List<string>? DependsOn { get; set; }

    [YamlMember(Alias = "deploy")]
    public Deploy? Deploy { get; set; }
}
