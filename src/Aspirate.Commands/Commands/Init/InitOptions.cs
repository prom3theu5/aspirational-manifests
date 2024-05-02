using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Commands.Init;

public sealed class InitOptions : BaseCommandOptions, IInitOptions, IContainerOptions
{
    public string? ProjectPath { get; set; }

    public string? ContainerBuilder { get; set; }

    public string? ContainerRegistry { get; set; }

    public string? ContainerRepositoryPrefix { get; set; }

    public string? ContainerImageTag { get; set; }
    public string? TemplatePath { get; set; }
}
