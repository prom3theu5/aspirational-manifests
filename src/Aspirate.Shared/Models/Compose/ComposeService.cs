namespace Aspirate.Shared.Models.Compose;

public class ComposeService : Service
{
    [YamlMember(Alias = "network_mode")]
    public string? NetworkMode { get; set; }
}
