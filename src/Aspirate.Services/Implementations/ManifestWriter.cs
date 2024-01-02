namespace Aspirate.Services.Implementations;

public class ManifestWriter(IFileSystem fileSystem) : IManifestWriter
{
    /// <summary>
    /// Mapping of template literals to corresponding template file names.
    /// </summary>
    private readonly Dictionary<string, string> _templateFileMapping = new()
    {
        [TemplateLiterals.DeploymentType] = $"{TemplateLiterals.DeploymentType}.hbs",
        [TemplateLiterals.DaprComponentType] = $"{TemplateLiterals.DaprComponentType}.hbs",
        [TemplateLiterals.ServiceType] = $"{TemplateLiterals.ServiceType}.hbs",
        [TemplateLiterals.ComponentKustomizeType] = $"{TemplateLiterals.ComponentKustomizeType}.hbs",
        [TemplateLiterals.RedisType] = $"{TemplateLiterals.RedisType}.hbs",
        [TemplateLiterals.SqlServerType] = $"{TemplateLiterals.SqlServerType}.hbs",
        [TemplateLiterals.MysqlServerType] = $"{TemplateLiterals.MysqlServerType}.hbs",
        [TemplateLiterals.RabbitMqType] = $"{TemplateLiterals.RabbitMqType}.hbs",
        [TemplateLiterals.MongoDbServerType] = $"{TemplateLiterals.MongoDbServerType}.hbs",
        [TemplateLiterals.PostgresServerType] = $"{TemplateLiterals.PostgresServerType}.hbs",
        [TemplateLiterals.NamespaceType] = $"{TemplateLiterals.NamespaceType}.hbs",
    };

    /// <summary>
    /// The default path to the template folder.
    /// </summary>
    private readonly string _defaultTemplatePath = Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder);

    /// <inheritdoc />
    public void EnsureOutputDirectoryExistsAndIsClean(string outputPath)
    {
        if (fileSystem.Directory.Exists(outputPath))
        {
            fileSystem.Directory.Delete(outputPath, true);
        }

        fileSystem.Directory.CreateDirectory(outputPath);
    }

    /// <inheritdoc />
    public void CreateDeployment<TTemplateData>(string outputPath, TTemplateData data, string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.DeploymentType, out var templateFile);
        var deploymentOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.DeploymentType}.yml");

        CreateFile(templateFile, deploymentOutputPath, data, templatePath);
    }

    public void CreateDaprManifest<TTemplateData>(string outputPath, TTemplateData data, string name, string? templatePath)
    {
        var daprOutputPath = Path.Combine(outputPath, "dapr");

        if (!fileSystem.Directory.Exists(daprOutputPath))
        {
            fileSystem.Directory.CreateDirectory(daprOutputPath);
        }

        _templateFileMapping.TryGetValue(TemplateLiterals.DaprComponentType, out var templateFile);
        var daprFileOutputPath = Path.Combine(daprOutputPath, $"{name}.yml");

        CreateFile(templateFile, daprFileOutputPath, data, templatePath);
    }

    /// <inheritdoc />
    public void CreateService<TTemplateData>(string outputPath, TTemplateData data, string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ServiceType, out var templateFile);
        var serviceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ServiceType}.yml");

        CreateFile(templateFile, serviceOutputPath, data, templatePath);
    }

    /// <inheritdoc />
    public void CreateComponentKustomizeManifest<TTemplateData>(
        string outputPath,
        TTemplateData data,
        string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.ComponentKustomizeType, out var templateFile);
        var kustomizeOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.ComponentKustomizeType}.yml");

        CreateFile(templateFile, kustomizeOutputPath, data, templatePath);
    }

    /// <inheritdoc />
    public void CreateNamespace<TTemplateData>(
        string outputPath,
        TTemplateData data,
        string? templatePath)
    {
        _templateFileMapping.TryGetValue(TemplateLiterals.NamespaceType, out var templateFile);
        var namespaceOutputPath = Path.Combine(outputPath, $"{TemplateLiterals.NamespaceType}.yml");

        CreateFile(templateFile, namespaceOutputPath, data, templatePath);
    }

    /// <inheritdoc />
    public void CreateCustomManifest<TTemplateData>(
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

    /// <inheritdoc />
    public void CreateImagePullSecret(string registryUrl, string registryUsername, string registryPassword, string registryEmail, string secretName, string outputPath)
    {
        var dockerConfigJson = CreateDockerConfigJson(registryUrl, registryUsername, registryPassword, registryEmail);

        var secret = ImagePullSecret.Create()
            .WithName(secretName)
            .WithDockerConfigJson(dockerConfigJson);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        string secretYaml = serializer.Serialize(secret);

        fileSystem.File.WriteAllText(fileSystem.Path.Combine(outputPath, $"{TemplateLiterals.ImagePullSecretType}.yml"), secretYaml);
    }

    private void CreateFile<TTemplateData>(string inputFile, string outputPath, TTemplateData data, string? templatePath)
    {
        var templateFile = GetTemplateFilePath(inputFile, templatePath);

        var template = fileSystem.File.ReadAllText(templateFile);
        var handlebarTemplate = Handlebars.Compile(template);
        var output = handlebarTemplate(data);

        fileSystem.File.WriteAllText(outputPath, output);
    }

    private string GetTemplateFilePath(string templateFile, string? templatePath) =>
        fileSystem.Path.Combine(templatePath ?? _defaultTemplatePath, templateFile);

    private static DockerConfigJson CreateDockerConfigJson(string registryUrl, string registryUsername, string registryPassword, string registryEmail)
    {
        string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{registryUsername}:{registryPassword}"));

        var dockerConfigJson = new DockerConfigJson
        {
            Auths = new()
            {
                [registryUrl] = new()
                {
                    Auth = auth,
                    Email = registryEmail,
                },
            },
        };

        return dockerConfigJson;
    }
}
