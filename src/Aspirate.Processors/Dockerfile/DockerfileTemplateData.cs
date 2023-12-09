namespace Aspirate.Processors.Dockerfile;

public class DockerfileTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string>? env,
    Dictionary<string, string>? secrets,
    List<Ports> ports,
    IReadOnlyCollection<string> manifests,
    string imagePullPolicy)
    : BaseTemplateData(name, env, secrets, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
    public bool IsDockerfile { get; set; } = true;
    public string ImagePullPolicy { get; set; } = imagePullPolicy;
    public List<Ports> Ports { get; set; } = ports;
    public bool HasPorts => Ports.Any();

    public string ServiceType { get; set; } = "ClusterIP";
}
