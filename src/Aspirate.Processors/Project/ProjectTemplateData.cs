namespace Aspirate.Processors.Project;

public class ProjectTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string>? env,
    Dictionary<string, string>? secrets,
    IReadOnlyCollection<string> manifests,
    string imagePullPolicy)
    : BaseTemplateData(name, env, secrets, manifests)
{
    public string ContainerImage { get; set; } = containerImage.ToLowerInvariant();
    public bool IsProject { get; set; } = true;
    public string ServiceType { get; set; } = "ClusterIP";
    public string ImagePullPolicy { get; set; } = imagePullPolicy;
}
