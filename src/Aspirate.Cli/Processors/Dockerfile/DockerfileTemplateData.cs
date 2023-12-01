namespace Aspirate.Cli.Processors.Dockerfile;

public class DockerfileTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    List<Ports> ports,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
    public bool IsDockerfile { get; set; } = true;
    public List<Ports> Ports { get; set; } = ports;
    public bool HasPorts => Ports.Any();

    public string ServiceType { get; set; } = "ClusterIP";
}
