namespace Aspirate.Contracts.Literals;

[ExcludeFromCodeCoverage]
public static class ContainerBuilderLiterals
{
    public const string ContainerRegistry = "ContainerRegistry";
    public const string ContainerRepository = "ContainerRepository";
    public const string ContainerImageName = "ContainerImageName";
    public const string ContainerImageTag = "ContainerImageTag";

    public const string DockerLoginCommand = "echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin $DOCKER_HOST";
}
