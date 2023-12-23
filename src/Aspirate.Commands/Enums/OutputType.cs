namespace Aspirate.Commands.Enums;

public class OutputFormat : SmartEnum<OutputFormat, string>
{
    private OutputFormat(string name, string value) : base(name, value)
    {
    }

    public static OutputFormat Kustomize = new OutputFormat(nameof(Kustomize), "kustomize");
    public static OutputFormat DockerCompose = new OutputFormat(nameof(DockerCompose), "compose");
}
