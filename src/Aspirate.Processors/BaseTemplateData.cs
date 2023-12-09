namespace Aspirate.Processors;

[ExcludeFromCodeCoverage]
public abstract class BaseTemplateData(
    string? name,
    Dictionary<string, string>? env,
    Dictionary<string, string>? secrets,
    IReadOnlyCollection<string>? manifests,
    bool isService = true)
{
    public string? Name { get; set; } = name;

    public Dictionary<string, string>? Env { get; set; } = env;
    public Dictionary<string, string>? Secrets { get; set; } = secrets;

    public IReadOnlyCollection<string>? Manifests { get; set; } = manifests;

    public bool IsService { get; set; } = isService;
}
