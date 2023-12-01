namespace Aspirate.Cli.Processors.Project;

public class ProjectTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
    public bool IsProject { get; set; } = true;
    public string ServiceType { get; set; } = "ClusterIP";
}
