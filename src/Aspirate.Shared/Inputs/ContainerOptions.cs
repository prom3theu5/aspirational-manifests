namespace Aspirate.Shared.Inputs;

public class ContainerOptions
{
    public string ContainerBuilder { get; set; } = default!;
    public Dictionary<string, string>? BuildArgs { get; set; }
    public string? BuildContext { get; set; }
    public string ImageName { get; set; } = default!;
    public string Registry { get; set; } = default!;
    public string? Prefix { get; set; }
    public List<string>? Tags { get; set; } = ["latest"];
}
