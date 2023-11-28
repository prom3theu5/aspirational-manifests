namespace Aspirate.Cli.Processors.Dockerfile;

public class DockerfileTemplateData(
    string name,
    string containerImage,
    Dictionary<string, string> env,
    Dictionary<int, int> ports,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, manifests)
{
    public string ContainerImage { get; set; } = containerImage;
}
