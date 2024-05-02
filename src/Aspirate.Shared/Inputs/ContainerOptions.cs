namespace Aspirate.Shared.Inputs;

public class ContainerOptions
{
    public string ContainerBuilder { get; set; } = default!;
    public string ImageName { get; set; } = default!;
    public string Registry { get; set; } = default!;
    public string? Prefix { get; set; }
    public string? Tag { get; set; }
}
