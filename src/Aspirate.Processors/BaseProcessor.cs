using AspireContainer = Aspirate.Shared.Models.AspireManifests.Components.V0.Container;

namespace Aspirate.Processors;

/// <summary>
/// Base class for all manifest handlers.
/// </summary>
public abstract partial class BaseProcessor<TTemplateData> : IProcessor where TTemplateData : BaseTemplateData
{
    /// <summary>
    /// A protected variable that holds an instance of the IFileSystem interface.
    /// </summary>
    /// <remarks>
    /// The IFileSystem interface provides methods for interacting with the file system.
    /// This variable is declared as protected to allow access within the class and its derived classes.
    /// </remarks>
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Represents a protected and read-only instance of the <see cref="IAnsiConsole"/> interface.
    /// </summary>
    protected readonly IAnsiConsole _console;

    /// <summary>
    /// The default path to the template folder.
    /// </summary>
    private readonly string _defaultTemplatePath = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder);

    /// <summary>
    /// Retrieves the type handlers used in the application.
    /// </summary>
    /// <returns>
    /// A dictionary that maps type names to their respective handlers.
    /// The keys are strings representing the type names, and the values
    /// are functions that take a string parameter and return a string.
    /// </returns>
    private readonly Dictionary<string, Func<string, Dictionary<string, Resource>, string>> _typeHandlers = new()
    {
        [AspireComponentLiterals.Redis] = (_, _) => "redis",
        [AspireComponentLiterals.PostgresDatabase] = (resourceName, _) => $"host=postgres-service;database={resourceName};username=postgres;password=postgres;",
        [AspireComponentLiterals.Container] = (resourceName, resources) => ReplaceConnectionStringPlaceholders(resources[resourceName] as AspireContainer, resources),
        [AspireComponentLiterals.RabbitMq] = (_, _) => "amqp://guest:guest@rabbitmq-service:5672",
        [AspireComponentLiterals.SqlServer] = (resourceName, resources) => $"Server=sqlserver-service,1433;User ID=sa;Password={resources[resourceName].Env["SaPassword"]};TrustServerCertificate=true;",
        [AspireComponentLiterals.MySqlServer] = (resourceName, resources) => $"Server=mysql-service;Port=3306;User ID=root;Password={resources[resourceName].Env["RootPassword"]};",
        [AspireComponentLiterals.MongoDbServer] = (_, _) => "mongodb://mongo-service:27017",
    };

    /// <summary>
    /// Retrieves the binding handlers used for constructing URLs based on a service name.
    /// </summary>
    /// <returns>A dictionary of binding handlers where the key is the binding name and the value is a function that takes a service name as input and returns the constructed URL.</returns>
    private readonly Dictionary<string, Func<string, string>> _bindingHandlers = new()
    {
        ["bindings.http.url"] = serviceName => $"http://{serviceName}:8080",
        ["bindings.https.url"] = serviceName => $"https://{serviceName}:8443",
    };

    /// <summary>
    /// List of environment variables that are considered protected and will be managed by secrets.
    /// </summary>
    private readonly List<string> _protectedEnvVars =
    [
        ProtectableLiterals.ConnectionString,
    ];

    /// <summary>
    /// Mapping of template literals to corresponding template file names.
    /// </summary>
    private readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = $"{TemplateLiterals.DeploymentType}.hbs",
        [TemplateLiterals.ServiceType] = $"{TemplateLiterals.ServiceType}.hbs",
        [TemplateLiterals.ComponentKustomizeType] = $"{TemplateLiterals.ComponentKustomizeType}.hbs",
        [TemplateLiterals.RedisType] = $"{TemplateLiterals.RedisType}.hbs",
        [TemplateLiterals.SqlServerType] = $"{TemplateLiterals.SqlServerType}.hbs",
        [TemplateLiterals.MysqlServerType] = $"{TemplateLiterals.MysqlServerType}.hbs",
        [TemplateLiterals.RabbitMqType] = $"{TemplateLiterals.RabbitMqType}.hbs",
        [TemplateLiterals.PostgresServerType] = $"{TemplateLiterals.PostgresServerType}.hbs",
        [TemplateLiterals.NamespaceType] = $"{TemplateLiterals.NamespaceType}.hbs",
    };

    /// <summary>
    /// Represents the base processor class for handling template data.
    /// </summary>
    protected BaseProcessor(IFileSystem fileSystem, IAnsiConsole console)
    {
        _fileSystem = fileSystem;
        _console = console;
    }

    /// <inheritdoc />
    public abstract string ResourceType { get; }

    /// <summary>
    /// Deserializes JSON data from the provided <see cref="Utf8JsonReader"/> into a <see cref="Resource"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> containing the JSON data.</param>
    /// <returns>The deserialized <see cref="Resource"/> object, or null if the deserialization fails.</returns>
    public abstract Resource? Deserialize(ref Utf8JsonReader reader);

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the filtered variables.
    /// </summary>
    /// <param name="resource">The resource whose environmental variables need to be filtered.</param>
    /// <returns>A dictionary representing the filtered environmental variables.</returns>
    protected Dictionary<string, string> GetFilteredEnvironmentalVariables(Resource resource)
    {
        var envVars = resource.Env;

        return envVars == null ? [] : envVars.Where(e => !_protectedEnvVars.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Filters the environmental variables of a given resource and returns a dictionary of the secret variables.
    /// </summary>
    /// <param name="resource">The resource from which to retrieve the secret environmental variables.</param>
    /// <returns>A dictionary representing the secret environmental variables.</returns>
    protected Dictionary<string, string> GetSecretEnvironmentalVariables(Resource resource)
    {
        var envVars = resource.Env;

        return envVars == null ? [] : envVars.Where(e => _protectedEnvVars.Any(p => e.Key.StartsWith(p))).ToDictionary(e => e.Key, e => e.Value);
    }

    /// <summary>
    /// Replaces placeholder values in the given resource.
    /// </summary>
    /// <param name="resource">The resource to replace placeholders in.</param>
    /// <param name="resources">A dictionary of resources.</param>
    public virtual void ReplacePlaceholders(Resource resource, Dictionary<string, Resource> resources)
    {
        if (resource.Env == null)
        {
            return;
        }

        foreach (var key in resource.Env.Keys)
        {
            var value = resource.Env[key];

            if (!value.StartsWith('{') || !value.EndsWith('}'))
            {
                continue;
            }

            var parts = value.Trim('{', '}').Split('.');
            var resourceName = parts[0];
            var resourceType = resources[resourceName].Type;
            var propertyPath = string.Join('.', parts.Skip(1));

            if (_typeHandlers.TryGetValue(resourceType, out var typeHandler))
            {
                resource.Env[key] = typeHandler(resourceName, resources);
                continue;
            }

            if (_bindingHandlers.TryGetValue(propertyPath, out var bindingHandler))
            {
                resource.Env[key] = bindingHandler(resourceName);
            }
        }
    }

    /// <summary>
    /// Creates manifests for a resource.
    /// </summary>
    /// <param name="resource">The key-value pair representing the resource and its name.</param>
    /// <param name="outputPath">The path where the manifests will be created.</param>
    /// <param name="imagePullPolicy">The image pull policy for the resource.</param>
    /// <param name="templatePath">Optional. The path to the template used for creating the manifests.</param>
    /// <param name="disableSecrets">Passing this will disable all secret generation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating if the manifests were created successfully.</returns>
    public virtual Task<bool> CreateManifests(KeyValuePair<string, Resource> resource, string outputPath, string imagePullPolicy,
        string? templatePath = null,
        bool? disableSecrets = false)
    {
        _console.LogCreateManifestNotOverridden(GetType().Name);

        return Task.FromResult(false);
    }

    /// <summary>
    /// Ensures that the output directory exists and is clean by deleting it if it exists
    /// and creating a new directory.
    /// </summary>
    /// <param name="outputPath">The path of the output directory</param>
    protected void EnsureOutputDirectoryExistsAndIsClean(string outputPath)
    {
        if (_fileSystem.Directory.Exists(outputPath))
        {
            _fileSystem.Directory.Delete(outputPath, true);
        }

        _fileSystem.Directory.CreateDirectory(outputPath);
    }

    /// <summary>
    /// Create a deployment file using the specified template file and template data.
    /// </summary>
    /// <param name="outputPath">The path where the deployment file will be created.</param>
    /// <param name="data">The template data.</param>
    /// <param name="templatePath">The path of the template file (optional).</param>
    protected void CreateDeployment(string outputPath, TTemplateData data, string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.DeploymentType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.DeploymentType}.yml");

        CreateFile(templateFile, deploymentOutputPath, data, templatePath);
    }

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
    protected void CreateService(string outputPath, TTemplateData data, string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ServiceType}.yml");

        CreateFile(templateFile, serviceOutputPath, data, templatePath);
    }

    /// <summary>
    /// Creates a Kustomize manifest for a component.
    /// </summary>
    /// <param name="outputPath">The directory where the manifest file will be created.</param>
    /// <param name="data">The data used to populate the template.</param>
    /// <param name="templatePath">The path to the template file (optional).</param>
    protected void CreateComponentKustomizeManifest(
        string outputPath,
        TTemplateData data,
        string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ComponentKustomizeType}.yml");

        CreateFile(templateFile, kustomizeOutputPath, data, templatePath);
    }

    protected void CreateNamespace(
        string outputPath,
        TTemplateData data,
        string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.NamespaceType, out var templateFile);
        var namespaceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.NamespaceType}.yml");

        CreateFile(templateFile, namespaceOutputPath, data, templatePath);
    }

    /// <summary>
    /// Create a custom manifest file using the specified parameters.
    /// </summary>
    /// <param name="outputPath">The output path of the manifest file.</param>
    /// <param name="fileName">The name of the manifest file.</param>
    /// <param name="templateType">The type of template to be used for creating the manifest.</param>
    /// <param name="data">The data object used for populating the template.</param>
    /// <param name="templatePath">The optional path to the template file.</param>
    protected void CreateCustomManifest(
        string outputPath,
        string fileName,
        string templateType,
        TTemplateData data,
        string? templatePath)
    {
        _templateFileMapping.TryGetValue(templateType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, fileName);

        CreateFile(templateFile, deploymentOutputPath, data, templatePath);
    }

    /// <summary>
    /// Creates a file using a template and provided data.
    /// </summary>
    /// <param name="inputFile">The input file path.</param>
    /// <param name="outputPath">The output file path.</param>
    /// <param name="data">The data to be used with the template.</param>
    /// <param name="templatePath">Optional. The path to the template file. If not provided, the method will use a default template.</param>
    private void CreateFile(string inputFile, string outputPath, TTemplateData data, string? templatePath)
    {
        var templateFile = GetTemplateFilePath(inputFile, templatePath);

        var template = _fileSystem.File.ReadAllText(templateFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        _fileSystem.File.WriteAllText(outputPath, output);
    }

    /// <summary>
    /// Gets the full file path of a template file.
    /// </summary>
    /// <param name="templateFile">The name of the template file.</param>
    /// <param name="templatePath">The optional custom template path. If not provided, the default template path will be used.</param>
    /// <returns>The full file path of the template file.</returns>
    private string GetTemplateFilePath(string templateFile, string? templatePath) =>
        _fileSystem.Path.Combine(templatePath ?? _defaultTemplatePath, templateFile);

    /// <summary>
    /// Logs the completion of a task with the given output path.
    /// </summary>
    /// <param name="outputPath">The path of the output file or directory.</param>
    protected void LogCompletion(string outputPath) =>
        _console.LogCompletion(_fileSystem.GetFullPath(outputPath));

    /// <summary>
    /// Replaces placeholders in a connection string with actual values from a container and resources dictionary.
    /// </summary>
    /// <param name="container">The AspireContainer object containing the connection string.</param>
    /// <param name="resources">The dictionary of resources used for replacing placeholders.</param>
    /// <returns>The connection string with replaced placeholders.</returns>
    protected static string ReplaceConnectionStringPlaceholders(AspireContainer container, IReadOnlyDictionary<string, Resource> resources) =>
        ConnectionStringRegex().Replace(container.ConnectionString, match =>
        {
            string[] pathParts = match.Groups[1].Value.Split('.');
            if (!resources.TryGetValue(pathParts[0], out var resource) || resource is not AspireContainer targetContainer)
            {
                throw new ArgumentException($"Resource {pathParts[0]} not found or is not a container.");
            }

            return pathParts[1] switch
            {
                "bindings" when pathParts[3] == "host"
                    => pathParts[0],  // return the name of the resource for 'host'

                "bindings" when pathParts[3] == "port" && targetContainer.Bindings != null && targetContainer.Bindings.TryGetValue(pathParts[2], out var binding)
                    => binding.ContainerPort.ToString(),

                "inputs" when targetContainer.Inputs != null && targetContainer.Inputs.TryGetValue(pathParts[2], out var input)
                    => input.Value ?? string.Empty,

                _ => throw new ArgumentException($"Unknown dictionary in placeholder {match.Value}."),
            };
        });

    /// <summary>
    /// Sets the secrets from the secret state for the specified resource using the provided secret provider.
    /// </summary>
    /// <param name="secrets">A dictionary containing the secrets.</param>
    /// <param name="resource">A key-value pair representing the resource.</param>
    /// <param name="secretProvider">An instance of the secret provider.</param>
    protected void SetSecretsFromSecretState(Dictionary<string, string> secrets, KeyValuePair<string, Resource> resource, ISecretProvider secretProvider)
    {
        if (!secretProvider.ResourceExists(resource.Key))
        {
            return;
        }

        for (int i = 0; i < secrets.Count; i++)
        {
            var secret = secrets.ElementAt(i);

            if (!secretProvider.SecretExists(resource.Key, secret.Key))
            {
                continue;
            }

            var encryptedSecret = secretProvider.GetSecret(resource.Key, secret.Key);

            secrets[secret.Key] = encryptedSecret;
        }
    }

    /// <summary>
    /// Gets the regular expression pattern used to extract the connection string key from a connection string.
    /// </summary>
    /// <returns>A <see cref="Regex"/> object representing the regular expression pattern.</returns>
    [GeneratedRegex(@"\{([\w\.]+)\}")]
    private static partial Regex ConnectionStringRegex();
}
