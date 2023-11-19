namespace Aspirational.Manifests.Mapping;

/// <summary>
/// Contains the mapping of aspire resource types to handlers
/// </summary>
public static class HandlerMapping
{
    /// <summary>
    /// The mapping of aspire resource types to handlers
    /// </summary>
    public static readonly Dictionary<string, BaseHandler> ResourceTypeToHandlerMap = new()
    {
        [AspireResourceLiterals.PostgresServer] = new PostgresServerHandler(),
        [AspireResourceLiterals.PostgresDatabase] = new PostgresDatabaseHandler(),
        [AspireResourceLiterals.Project] = new ProjectHandler(),
        [AspireResourceLiterals.Final] = new FinalHandler(),
        // Add more as needed
    };


    public static readonly Dictionary<string, string> TemplateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "deployment.hbs"),
        [TemplateLiterals.ServiceType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "service.hbs"),
        [TemplateLiterals.ComponentKustomizeType] = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder, "kustomization.hbs"),
        // Add more as needed
    };
}
