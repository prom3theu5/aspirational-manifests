namespace Aspirate.Cli.Processors.Components.Project;

public class ProjectTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<int> containerPorts,
    IReadOnlyCollection<string> manifests)
    : BaseTemplateData(name, env, containerPorts, manifests, true);
