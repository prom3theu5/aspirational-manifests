namespace Aspirate.Shared.Models.Aspirate;

public class AspirateContainerSettings
{
    public string? Registry { get; set; }
    public string? RepositoryPrefix { get; set; }
    public List<string>? Tags { get; set; }
    public string? Builder { get; set; }
    public List<string>? BuildArgs { get; set; }
    public string? Context { get; set; }
}
