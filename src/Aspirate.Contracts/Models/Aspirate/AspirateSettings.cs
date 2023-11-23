namespace Aspirate.Contracts.Models.Aspirate;

public class AspirateSettings
{
    public const string FileName = "aspirate.json";

    public string? TemplatePath { get; set; }

    public AspirateContainerSettings? ContainerSettings { get; set; } = new();
}
