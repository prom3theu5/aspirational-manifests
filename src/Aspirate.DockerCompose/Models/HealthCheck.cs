namespace Aspirate.DockerCompose.Models;

[Serializable]
public class HealthCheck
{
    [YamlMember(Alias = "test", ScalarStyle = ScalarStyle.Folded)]
    public string[] TestCommand { get; set; } = null!;

    [YamlMember(Alias = "interval")]
    public string? Interval { get; set; }

    [YamlMember(Alias = "timeout")]
    public string? Timeout { get; set; }

    [YamlMember(Alias = "retries")]
    public int? Retries { get; set; }

    [YamlMember(Alias = "start_period")]
    public string? StartPeriod { get; set; }
}
