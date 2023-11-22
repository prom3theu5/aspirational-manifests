namespace Aspirate.Contracts.Literals;

public static class ContainerBuilderLiterals
{
    public const string ContainerRegistry = "ContainerRegistry";
    public const string ContainerRepository = "ContainerRepository";
    public const string ContainerImageName = "ContainerImageName";
    public const string ContainerImageTag = "ContainerImageTag";

    public const string DotNetCommand = "dotnet";
    public const string DockerLoginCommand = "echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin $DOCKER_HOST";
    public static string DefaultBuildArguments(string projectPath, string os = "linux", string arch = "x64") =>
        $"publish {projectPath} --os {os} --arch {arch} -p:PublishProfile=DefaultContainer -p:PublishSingleFile=true --self-contained true";
    public static string DuplicateFileOutputBuildArguments(string projectPath, string os = "linux", string arch = "x64") =>
        $"publish {projectPath} --os {os} --arch {arch} -p:PublishProfile=DefaultContainer -p:PublishSingleFile=true --self-contained true -p:ErrorOnDuplicatePublishOutputFiles=false";
}
