namespace Aspirate.Commands.Contracts;

public interface IInitOptions
{
    string? ProjectPath { get; set; }

    string? ContainerRegistry { get; set; }

    string? ContainerImageTag { get; set; }

    string? TemplatePath { get; set; }
}
