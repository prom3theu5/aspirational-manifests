namespace Aspirate.Shared.Inputs;

public abstract class BaseCreateOptions
{
    /// <summary>
    /// Represents a resource in a manifest.
    /// </summary>
    public KeyValuePair<string, Resource> Resource { get; set; }

    /// <summary>
    /// Specifies whether the resource should include a dashboard.
    /// </summary>
    public bool? WithDashboard { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether secrets should be disabled.
    /// </summary>
    public bool? DisableSecrets { get; set; }

    /// <summary>
    /// Gets or sets the flag indicating whether private registry should be used.
    /// </summary>
    /// <remarks>
    /// By default, the private registry is not used. If this flag is set to <see langword="true"/>, the application will pull Docker images from a private registry instead of the public Docker Hub.
    /// </remarks>
    public bool? WithPrivateRegistry { get; set; }

    public AspirateState? CurrentState { get; set; }
}
