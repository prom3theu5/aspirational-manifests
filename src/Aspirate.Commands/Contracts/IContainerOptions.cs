namespace Aspirate.Commands.Contracts;

public interface IContainerOptions
{
    string? ContainerBuilder { get; set; }

    string? ContainerRegistry { get; set; }
    string? ContainerRepositoryPrefix { get; set; }

    string? ContainerImageTag { get; set; }
}
