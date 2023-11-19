namespace Aspirate.Models;

public abstract class BaseTemplateData(
    string? name,
    Dictionary<string, string>? env,
    IReadOnlyCollection<int>? containerPorts,
    IReadOnlyCollection<string>? manifests,
    bool isService = true)
{
    public string? Name { get; set; } = name;

    public Dictionary<string, string>? Env { get; set; } = env;

    public IReadOnlyCollection<int>? ContainerPorts { get; set; } = containerPorts;

    public IReadOnlyCollection<string>? Manifests { get; set; } = manifests;

    public bool IsService { get; set; } = isService;
}
