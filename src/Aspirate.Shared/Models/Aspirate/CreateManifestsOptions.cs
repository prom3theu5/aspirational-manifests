namespace Aspirate.Shared.Models.Aspirate;

/// <summary>
/// Represents options for creating manifests.
/// </summary>
public sealed class CreateManifestsOptions
{
    /// <summary>
    /// A resource in a manifest.
    /// </summary>
    public KeyValuePair<string, Resource> Resource { get; set; }

    /// <summary>
    /// Gets or sets the output path for the manifest.
    /// </summary>
    /// <remarks>
    /// This property specifies the directory path where the manifest will be saved.
    /// </remarks>
    public required string OutputPath { get; set; }

    /// <summary>
    /// Specifies the image pull policy for a resource in a manifest.
    /// </summary>
    public required string ImagePullPolicy { get; set; }

    /// <summary>
    /// Gets or sets the path to the template associated with the resource.
    /// </summary>
    /// <remarks>
    /// This property is used to specify the path to the template file that is associated with the resource.
    /// It is applicable to various resource types such as ProjectResource, DockerfileResource, ContainerResource, etc.
    /// The template file contains the configuration and settings for the resource.
    /// </remarks>
    public string? TemplatePath { get; set; }

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

    /// <summary>
    /// Gets or sets a value indicating whether the dashboard is enabled.
    /// </summary>
    /// <remarks>
    /// When the dashboard is enabled, additional information and controls are available for monitoring and managing the application.
    /// </remarks>
    public bool? WithDashboard { get; set; }
}
