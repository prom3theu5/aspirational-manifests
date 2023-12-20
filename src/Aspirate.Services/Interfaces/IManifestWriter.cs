namespace Aspirate.Services.Interfaces;

public interface IManifestWriter
{
    /// <summary>
    /// Ensures that the output directory exists and is clean by deleting it if it exists
    /// and creating a new directory.
    /// </summary>
    /// <param name="outputPath">The path of the output directory</param>
    void EnsureOutputDirectoryExistsAndIsClean(string outputPath);

    /// <summary>
    /// Create a deployment file using the specified template file and template data.
    /// </summary>
    /// <param name="outputPath">The path where the deployment file will be created.</param>
    /// <param name="data">The template data.</param>
    /// <param name="templatePath">The path of the template file (optional).</param>
    void CreateDeployment<TTemplateData>(string outputPath, TTemplateData data, string? templatePath);

    /// <summary>
    /// Creates a service based on the specified output path, template data, and optional template path.
    /// </summary>
    /// <param name="outputPath">The path to where the service should be created.</param>
    /// <param name="data">The template data to be used for generating the service.</param>
    /// <param name="templatePath">The optional path to the template file.</param>
    /// <remarks>
    /// The service will be created using the specified template data. If a template path is provided, it will be used as the template file for generating the service.
    /// If no template path is provided, the default template file for the service type will be used. The generated service will be saved to the specified output path.
    /// </remarks>
    void CreateService<TTemplateData>(string outputPath, TTemplateData data, string? templatePath);

    /// <summary>
    /// Creates a Kustomize manifest for a component.
    /// </summary>
    /// <param name="outputPath">The directory where the manifest file will be created.</param>
    /// <param name="data">The data used to populate the template.</param>
    /// <param name="templatePath">The path to the template file (optional).</param>
    void CreateComponentKustomizeManifest<TTemplateData>(
        string outputPath,
        TTemplateData data,
        string? templatePath);

    void CreateNamespace<TTemplateData>(
        string outputPath,
        TTemplateData data,
        string? templatePath);

    /// <summary>
    /// Create a custom manifest file using the specified parameters.
    /// </summary>
    /// <param name="outputPath">The output path of the manifest file.</param>
    /// <param name="fileName">The name of the manifest file.</param>
    /// <param name="templateType">The type of template to be used for creating the manifest.</param>
    /// <param name="data">The data object used for populating the template.</param>
    /// <param name="templatePath">The optional path to the template file.</param>
    void CreateCustomManifest<TTemplateData>(
        string outputPath,
        string fileName,
        string templateType,
        TTemplateData data,
        string? templatePath);
}
