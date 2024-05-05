namespace Aspirate.Shared.Inputs;

/// <summary>
/// Represents options for creating manifests.
/// </summary>
public sealed class CreateManifestsOptions : BaseKubernetesCreateOptions
{
    /// <summary>
    /// Gets or sets the output path for the manifest.
    /// </summary>
    /// <remarks>
    /// This property specifies the directory path where the manifest will be saved.
    /// </remarks>
    public required string OutputPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the template associated with the resource.
    /// </summary>
    /// <remarks>
    /// This property is used to specify the path to the template file that is associated with the resource.
    /// It is applicable to various resource types such as ProjectResource, DockerfileResource, ContainerResource, etc.
    /// The template file contains the configuration and settings for the resource.
    /// </remarks>
    public string? TemplatePath { get; set; }
}
