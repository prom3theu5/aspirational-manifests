namespace Aspirate.Cli.Processors.Components.Project;

public class ProjectTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    IReadOnlyCollection<int> containerPorts,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, containerPorts, manifests)
{
    public string ContainerImage { get; set; } = containerImage;

    public bool HasContainerPorts => ContainerPorts?.Any() ?? false;
}
