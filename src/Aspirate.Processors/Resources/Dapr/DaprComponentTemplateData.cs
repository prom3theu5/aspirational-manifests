namespace Aspirate.Processors.Resources.Dapr;

public sealed class DaprComponentTemplateData : KubernetesDeploymentData
{
    public string Type { get; private set; } = default!;
    public string? Version { get; private set; }
    public bool HasMetadata { get; private set; }
    public bool EmptyMetadata { get; private set; }

    public Dictionary<string, string>? Metadata { get; private set; }

    public DaprComponentTemplateData SetType(string type)
    {
        Type = type;
        return this;
    }

    public DaprComponentTemplateData SetVersion(string? version)
    {
        Version = !string.IsNullOrWhiteSpace(version) ? version : "v1";
        return this;
    }

    public KubernetesDeploymentData SetMetadata(Dictionary<string, string>? metadata)
    {
        Metadata = metadata;

        if (Metadata is null || Metadata.Count == 0)
        {
            EmptyMetadata = true;
            HasMetadata = false;
            return this;
        }

        HasMetadata = true;
        EmptyMetadata = false;
        return this;
    }
}
