namespace Aspirate.Shared.Literals;

[ExcludeFromCodeCoverage]
public static class DotNetSdkLiterals
{
    public const string DuplicateFileOutputError = "NETSDK1152";
    public const string NoContainerRegistryAccess = "CONTAINER1016";
    public const string UnknownContainerRegistryAddress = "CONTAINER1013";

    public const string MsBuildArgument = "msbuild";
    public const string RunArgument = "run";
    public const string PublishArgument = "publish";
    public const string ArgumentDelimiter = "--";

    public const string ProjectArgument = "--project";
    public const string PublisherArgument = "--publisher";
    public const string OutputPathArgument = "--output-path";
    public const string SelfContainedArgument = "--self-contained";
    public const string RuntimeIdentifierArgument = "-r";
    public const string OsArgument = "--os";
    public const string ArchArgument = "--arch";
    public const string GetPropertyArgument = "--getProperty";

    public const string PublishProfileArgument = $"-p:{MsBuildPropertiesLiterals.PublishProfileArgument}";
    public const string PublishSingleFileArgument = $"-p:{MsBuildPropertiesLiterals.PublishSingleFileArgument}";
    public const string PublishTrimmedArgument = $"-p:{MsBuildPropertiesLiterals.PublishTrimmedArgument}";
    public const string ContainerRegistryArgument = $"-p:{MsBuildPropertiesLiterals.ContainerRegistryArgument}";
    public const string ContainerRepositoryArgument = $"-p:{MsBuildPropertiesLiterals.ContainerRepositoryArgument}";
    public const string ContainerImageNameArgument = $"-p:{MsBuildPropertiesLiterals.ContainerImageNameArgument}";
    public const string ContainerImageTagArgument = $"-p:{MsBuildPropertiesLiterals.ContainerImageTagArgument}";
    public const string ErrorOnDuplicatePublishOutputFilesArgument = $"-p:{MsBuildPropertiesLiterals.ErrorOnDuplicatePublishOutputFilesArgument}";

    public const string ContainerPublishProfile = "DefaultContainer";
    public const string DefaultSingleFile = "true";
    public const string DefaultSelfContained = "true";
    public const string DefaultPublishTrimmed = "false";
    public const string DefaultRuntimeIdentifier = "linux-x64";
    public const string DefaultOs = "linux";
    public const string DefaultArch = "x64";

    public const string DotNetCommand = "dotnet";

}
