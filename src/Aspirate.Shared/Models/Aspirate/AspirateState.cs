namespace Aspirate.Shared.Models.Aspirate;

public class AspirateState :
    IInitOptions,
    IGenerateOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IKubernetesOptions,
    IPrivateRegistryCredentialsOptions,
    IApplyOptions,
    IDashboardOptions
{
    [RestorableStateProperty]
    [JsonPropertyName("projectPath")]
    public string? ProjectPath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("inputPath")]
    public string? InputPath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("outputPath")]
    public string? OutputPath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("namespace")]
    public string? Namespace { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerRegistry")]
    public string? ContainerRegistry { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerImageTags")]
    public List<string>? ContainerImageTags { get; set; } = ["latest"];

    [RestorableStateProperty]
    [JsonPropertyName("runtimeIdentifier")]
    public string? RuntimeIdentifier { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("composeBuilds")]
    public List<string>? ComposeBuilds { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("imagePullPolicy")]
    public string? ImagePullPolicy { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerBuilder")]
    public string? ContainerBuilder { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("templatePath")]
    public string? TemplatePath { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("containerRepositoryPrefix")]
    public string? ContainerRepositoryPrefix { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("kubeContext")]
    public string? KubeContext { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("outputFormat")]
    public string? OutputFormat { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("disableSecrets")]
    public bool? DisableSecrets { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("skipFinalKustomizeGeneration")]
    public bool? SkipFinalKustomizeGeneration { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryUrl")]
    public string? PrivateRegistryUrl { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryUsername")]
    public string? PrivateRegistryUsername { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("privateRegistryEmail")]
    public string? PrivateRegistryEmail { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("withPrivateRegistry")]
    public bool? WithPrivateRegistry { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("rollingRestart")]
    public bool? RollingRestart { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("includeDashboard")]
    public bool? IncludeDashboard { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("useCustomNamespace")]
    public bool? UseCustomNamespace { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("secrets")]
    public SecretState? SecretState { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("processAllComponents")]
    public bool? ProcessAllComponents { get; set; }

    [RestorableStateProperty]
    [JsonPropertyName("existingSecretsType")]
    public ExistingSecretsType? ExistingSecretsType { get; set; }

    [JsonIgnore]
    public bool? SkipBuild { get; set; }

    [JsonIgnore]
    public string? AspireManifest { get; set; }

    [JsonIgnore]
    public List<string> AspireComponentsToProcess { get; set; } = new();

    [JsonIgnore]
    public Dictionary<string, Resource> LoadedAspireManifestResources { get; set; } = new();

    [JsonIgnore]
    public string? PrivateRegistryPassword { get; set; }

    [JsonIgnore]
    public Dictionary<string, Resource> FinalResources { get; } = new();

    [JsonIgnore]
    public bool StateWasLoadedFromPrevious { get; set; }

    [JsonIgnore]
    public bool UseAllPreviousStateValues { get; set; }

    [JsonIgnore]
    public bool NonInteractive { get; set; }

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

    [JsonIgnore]
    public string? SecretPassword { get; set; }

    public void AppendToFinalResources(string key, Resource resource) =>
        FinalResources.Add(key, resource);

    public static bool IsNotDeployable(Resource resource) =>
        (resource is DaprResource or ParameterResource or ValueResource);
}
