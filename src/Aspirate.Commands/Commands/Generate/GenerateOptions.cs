namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateOptions : BaseCommandOptions,
    IBuildOptions,
    IContainerOptions,
    IAspireOptions,
    IGenerateOptions,
    ISecretOption,
    IPrivateRegistryCredentialsOptions
{
    public string? ProjectPath { get; set; }
    public string? AspireManifest { get; set; }
    public string? OutputPath { get; set; }

    public string? Namespace { get; set; }

    public bool SkipBuild { get; set; }
    public bool SkipFinalKustomizeGeneration { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerImageTag { get; set; }

    public string? ImagePullPolicy { get; set; }

    public string? OutputFormat { get; set; }

    public string? RuntimeIdentifier { get; set; }

    public string? SecretPassword { get; set; }

    public string? RegistryUsername { get; set; }

    public string? RegistryPassword { get; set; }

    public string? RegistryEmail { get; set; }

    public bool? WithPrivateRegistry { get; set; }
}
