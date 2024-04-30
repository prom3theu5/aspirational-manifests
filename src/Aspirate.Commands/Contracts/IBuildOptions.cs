namespace Aspirate.Commands.Contracts;

public interface IBuildOptions
{
    string? RuntimeIdentifier { get; set; }
    bool? ComposeBuilds { get; set; }
}
