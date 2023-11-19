namespace Aspirate.ManifestHandlers.Components.Project;

public class ProjectTemplateData(
    string name,
    Dictionary<string, string> env,
    IReadOnlyCollection<int> containerPorts,
    IReadOnlyCollection<string> manifests,
    bool isService)
    : BaseTemplateData(name, env, containerPorts, manifests, isService);
