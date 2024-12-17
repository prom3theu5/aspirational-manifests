namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IContainerOptions
{
    string? ContainerBuilder { get; set; }
    string? ContainerBuildContext { get; set; }
    string? ContainerRegistry { get; set; }
    string? ContainerRepositoryPrefix { get; set; }
    List<string>? ContainerImageTags { get; set; }
    List<string>? ContainerBuildArgs { get; set; }
}
