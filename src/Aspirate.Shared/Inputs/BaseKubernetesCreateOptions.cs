namespace Aspirate.Shared.Inputs;

public abstract class BaseKubernetesCreateOptions : BaseCreateOptions
{
    /// <summary>
    /// Specifies the image pull policy for a resource in a manifest.
    /// </summary>
    public required string ImagePullPolicy { get; set; }
}
