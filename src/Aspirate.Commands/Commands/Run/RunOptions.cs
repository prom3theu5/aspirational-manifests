namespace Aspirate.Commands.Commands.Run;

public sealed class RunOptions : BaseCommandOptions,
    IContainerOptions,
    IAspireOptions,
    IPrivateRegistryCredentialsOptions,
    IDashboardOptions,
    IRunOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public bool? AllowClearNamespace { get; set; }
    public string? Namespace { get; set; }
    public bool? SkipBuild { get; set; }
    public string? ContainerBuilder { get; set; }
    public List<string>? ContainerBuildArgs { get; set; }
    public string? ContainerBuildContext { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerRepositoryPrefix { get; set; }
    public List<string>? ContainerImageTags { get; set; }
    public string? ImagePullPolicy { get; set; }
    public string? RuntimeIdentifier { get; set; }
    public string? PrivateRegistryUrl { get; set; }
    public string? PrivateRegistryUsername { get; set; }
    public string? PrivateRegistryPassword { get; set; }
    public string? PrivateRegistryEmail { get; set; }
    public bool? WithPrivateRegistry { get; set; }
    public bool? IncludeDashboard { get; set; }
}
