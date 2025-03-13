namespace Aspirate.Shared.Inputs;

/// <summary>
/// Represents options for creating Kubernetes Objects.
/// </summary>
public sealed class CreateKubernetesObjectsOptions : BaseKubernetesCreateOptions
{
    public bool EncodeSecrets { get; set; } = true;
}
