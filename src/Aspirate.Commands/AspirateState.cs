namespace Aspirate.Commands;

public class AspirateState :
    IApplyOptions,
    IDestroyOptions,
    IInitOptions,
    IGenerateOptions,
    IBuildOptions,
    IPasswordSecretState,
    IBase64SecretState
{
    [JsonPropertyName("projectPath")]
    public string ProjectPath { get; set; } = null!;

    [JsonPropertyName("inputPath")]
    public string InputPath { get; set; } = null!;

    [JsonPropertyName("outputPath")]
    public string OutputPath { get; set; } = null!;

    [JsonPropertyName("aspireManifest")]
    public string? AspireManifest { get; set; }

    [JsonPropertyName("containerRegistry")]
    public string? ContainerRegistry { get; set; }

    [JsonPropertyName("containerImageTag")]
    public string? ContainerImageTag { get; set; }

    [JsonPropertyName("imagePullPolicy")]
    public string? ImagePullPolicy { get; set; }

    [JsonPropertyName("containerBuilder")]
    public string? ContainerBuilder { get; set; }

    [JsonPropertyName("templatePath")]
    public string? TemplatePath { get; set; }

    [JsonPropertyName("kubeContext")]
    public string? KubeContext { get; set; }

    [JsonPropertyName("nonInteractive")]
    public bool NonInteractive { get; set; }

    [JsonPropertyName("secretProvider")]
    public ProviderType SecretProvider { get; set; }

    [JsonPropertyName("skipBuild")]
    public bool SkipBuild { get; set; }

    [JsonPropertyName("skipFinalKustomizeGeneration")]
    public bool SkipFinalKustomizeGeneration { get; set; }

    [JsonPropertyName("salt")]
    public string? Salt { get; set; }

    [JsonPropertyName("secrets")]
    public Dictionary<string, Dictionary<string, string>> Secrets { get; set; } = [];

    [JsonPropertyName("secretsVersion")]
    public int? Version { get; set; }

    public string? Hash { get; set; }

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
            .Where(x => x.Value is Project && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    [JsonIgnore]
    public List<KeyValuePair<string, Resource>> SelectedDockerfileComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is Dockerfile && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    [JsonIgnore]
    public List<KeyValuePair<string, Resource>> AllSelectedSupportedComponents =>
        LoadedAspireManifestResources
            .Where(x => x.Value is not UnsupportedResource && AspireComponentsToProcess.Contains(x.Key))
            .ToList();

    [JsonIgnore]
    public bool HasSelectedSupportedComponents => !AllSelectedSupportedComponents.All(x => IsDatabase(x.Value));

    public void AppendToFinalResources(string key, Resource resource) =>
        FinalResources.Add(key, resource);

    public bool IsDatabase(Resource resource) =>
        resource is PostgresDatabase;
}
