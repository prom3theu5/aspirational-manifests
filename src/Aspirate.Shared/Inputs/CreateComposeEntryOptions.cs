namespace Aspirate.Shared.Inputs;

public sealed class CreateComposeEntryOptions
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
    /// Represents the option to include Compose builds in a manifest.
    /// </summary>
    /// <remarks>
    /// Compose builds are used to build Docker images using Docker Compose files.
    /// </remarks>
    public bool? ComposeBuilds { get; set; }
}
