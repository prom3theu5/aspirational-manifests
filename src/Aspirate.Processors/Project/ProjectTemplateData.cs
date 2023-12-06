namespace Aspirate.Processors.Project;

public class ProjectTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    IReadOnlyCollection<string> manifests,
    string imagePullPolicy)
    : BaseTemplateData(name, env, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
    public bool IsProject { get; set; } = true;
    public string ServiceType { get; set; } = "ClusterIP";
    public string ImagePullPolicy { get; set; } = imagePullPolicy;
}
