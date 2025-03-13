namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateOptions : BaseCommandOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IGenerateOptions,
    IPrivateRegistryCredentialsOptions,
    IDashboardOptions,
    ISecretState,
    IComponentsOptions
    IKubernetesOptions,
    IMinikubeOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public string? OutputPath { get; set; }
    public string? Namespace { get; set; }
    public bool? SkipBuild { get; set; }
    public bool? SkipFinalKustomizeGeneration { get; set; }
    public string? ContainerBuilder { get; set; }
    public List<string>? ContainerBuildArgs { get; set; }
    public string? ContainerBuildContext { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerRepositoryPrefix { get; set; }
    public List<string>? ContainerImageTags { get; set; }
    public string? ImagePullPolicy { get; set; }
    public string? OutputFormat { get; set; }
    public List<string>? Parameters { get; set; }
    public string? RuntimeIdentifier { get; set; }
    public List<string>? ComposeBuilds { get; set; }
    public bool PreferDockerfile { get; set; }
    public string? PrivateRegistryUrl { get; set; }
    public string? PrivateRegistryUsername { get; set; }
    public string? PrivateRegistryPassword { get; set; }
    public string? PrivateRegistryEmail { get; set; }
    public bool? WithPrivateRegistry { get; set; }
    public bool? IncludeDashboard { get; set; }
    public bool? ReplaceSecrets { get; set; }
    public List<string>? CliSpecifiedComponents { get; set; }
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }
    public bool? EnableMinikubeMountAction { get; set; }
}
