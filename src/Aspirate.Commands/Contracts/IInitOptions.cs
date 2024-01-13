namespace Aspirate.Commands.Contracts;

public interface IInitOptions
{
    string? ProjectPath { get; set; }

    string? TemplatePath { get; set; }
}
