namespace Aspirate.Processors.Container;

public class ContainerTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string>? env,
    Dictionary<string, string>? secrets,
    List<Ports> ports,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, secrets, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
    public List<Ports> Ports { get; set; } = ports;

    public bool HasPorts => Ports.Any();

    public string ServiceType { get; set; } = "ClusterIP";
}
