namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateOptions : BaseCommandOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IGenerateOptions,
    ISecretOption,
    IPrivateRegistryCredentialsOptions,
    IDashboardOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public string? OutputPath { get; set; }

    public string? Namespace { get; set; }

    public bool SkipBuild { get; set; }
    public bool SkipFinalKustomizeGeneration { get; set; }
    public bool SkipHelmGeneration { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerRegistry { get; set; }

    public string? ContainerRepositoryPrefix { get; set; }

    public string? ContainerImageTag { get; set; }

    public string? ImagePullPolicy { get; set; }

    public string? OutputFormat { get; set; }

    public string? RuntimeIdentifier { get; set; }
    public bool? ComposeBuilds { get; set; }

    public string? SecretPassword { get; set; }

    public string? PrivateRegistryUrl { get; set; }

    public string? PrivateRegistryUsername { get; set; }

    public string? PrivateRegistryPassword { get; set; }

    public string? PrivateRegistryEmail { get; set; }

    public bool? WithPrivateRegistry { get; set; }
    public bool? IncludeDashboard { get; set; }
}
