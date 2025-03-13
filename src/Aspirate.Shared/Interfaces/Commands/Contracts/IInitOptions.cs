namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IInitOptions
{
    string? ProjectPath { get; set; }

    string? TemplatePath { get; set; }
}
