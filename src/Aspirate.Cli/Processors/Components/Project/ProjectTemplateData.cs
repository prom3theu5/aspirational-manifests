namespace Aspirate.Cli.Processors.Components.Project;

public class ProjectTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
}
