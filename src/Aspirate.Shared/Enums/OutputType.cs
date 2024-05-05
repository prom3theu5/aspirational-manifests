namespace Aspirate.Shared.Enums;

public class OutputFormat : SmartEnum<OutputFormat, string>
{
    private OutputFormat(string name, string value) : base(name, value)
    {
    }

    public static OutputFormat Kustomize = new(nameof(Kustomize), "kustomize");
    public static OutputFormat DockerCompose = new(nameof(DockerCompose), "compose");
}
