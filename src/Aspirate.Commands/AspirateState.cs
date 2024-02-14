namespace Aspirate.Commands;

public class AspirateState :
    IInitOptions,
    IGenerateOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IKubernetesOptions,
    ISecretOption,
    IPasswordSecretState,
    IBase64SecretState,
    IPrivateRegistryCredentialsOptions
{
    [JsonPropertyName("projectPath")]
    public string? ProjectPath { get; set; }

    [JsonPropertyName("inputPath")]
    public string? InputPath { get; set; }

    [JsonPropertyName("outputPath")]
    public string? OutputPath { get; set; } = null!;

    [JsonPropertyName("namespace")]
    public string? Namespace { get; set; }

    [JsonPropertyName("aspireManifest")]
    public string? AspireManifest { get; set; }

    [JsonPropertyName("containerRegistry")]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName("containerImageTag")]
    public string? ContainerImageTag { get; set; }

    [JsonPropertyName("runtimeIdentifier")]
    public string? RuntimeIdentifier { get; set; }

    [JsonPropertyName("imagePullPolicy")]
    public string? ImagePullPolicy { get; set; }

    [JsonPropertyName("containerBuilder")]
    public string? ContainerBuilder { get; set; }

    [JsonPropertyName("templatePath")]
    public string? TemplatePath { get; set; }

    [JsonPropertyName("containerRepositoryPrefix")]
    public string? ContainerRepositoryPrefix { get; set; }

    [JsonPropertyName("kubeContext")]
    public string? KubeContext { get; set; }

    [JsonPropertyName("nonInteractive")]
    public bool NonInteractive { get; set; }

    [JsonPropertyName("outputFormat")]
    public string? OutputFormat { get; set; }

    [JsonPropertyName("disableSecrets")]
    public bool DisableSecrets { get; set; }

    [JsonPropertyName("secretProvider")]
    public ProviderType SecretProvider { get; set; }

    [JsonPropertyName("skipBuild")]
    public bool SkipBuild { get; set; }

    [JsonPropertyName("skipFinalKustomizeGeneration")]
    public bool SkipFinalKustomizeGeneration { get; set; }

    [JsonPropertyName("privateRegistryUrl")]
    public string? PrivateRegistryUrl { get; set; }
    [JsonPropertyName("privateRegistryUsername")]
    public string? PrivateRegistryUsername { get; set; }

    [JsonPropertyName("privateRegistryPassword")]
    public string? PrivateRegistryPassword { get; set; }

    [JsonPropertyName("privateRegistryEmail")]
    public string? PrivateRegistryEmail { get; set; }

    [JsonPropertyName("withPrivateRegistry")]
    public bool? WithPrivateRegistry { get; set; }

    [JsonPropertyName("salt")]
    public string? Salt { get; set; }

    [JsonPropertyName("secrets")]
    public Dictionary<string, Dictionary<string, string>> Secrets { get; set; } = [];

    [JsonPropertyName("secretsVersion")]
    public int? Version { get; set; }

    public string? Hash { get; set; }

    public string? SecretPassword { get; set; }

    [JsonIgnore]
    public List<string> AspireComponentsToProcess { get; set; } = [];

    [JsonIgnore]
    public Dictionary<string, Resource> LoadedAspireManifestResources { get; set; } = [];

    [JsonIgnore]
    public Dictionary<string, Resource> FinalResources { get; } = [];

    [JsonIgnore]
    public bool ActiveKubernetesContextIsSet => !string.IsNullOrEmpty(KubeContext);

    [JsonIgnore]
    public List<KeyValuePair<string, Resource>> SelectedProjectComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is ProjectResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    [JsonIgnore]
    public List<KeyValuePair<string, Resource>> SelectedDockerfileComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is DockerfileResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    [JsonIgnore]
    public List<KeyValuePair<string, Resource>> AllSelectedSupportedComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is not UnsupportedResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

   [JsonIgnore]
    public bool HasSelectedSupportedComponents => !AllSelectedSupportedComponents.All(x => IsNotDeployable(x.Value));

    public void AppendToFinalResources(string key, Resource resource) =>
        FinalResources.Add(key, resource);

    public static bool IsNotDeployable(Resource resource) =>
        (resource is PostgresDatabaseResource
            or SqlServerDatabaseResource
            or MySqlDatabaseResource
            or MongoDbDatabaseResource
            or DaprResource)
        && !IsAzureResource(resource);


    private static bool IsAzureResource(Resource resource) =>
        resource is AzureKeyVaultResource
            or AzureStorageResource
            or AzureStorageBlobResource;
}
