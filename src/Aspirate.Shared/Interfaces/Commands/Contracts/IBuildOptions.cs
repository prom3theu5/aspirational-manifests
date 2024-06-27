namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IBuildOptions
{
    string? RuntimeIdentifier { get; set; }
    List<string>? ComposeBuilds { get; set; }
    bool? UseSecrets { get; set; }
}
