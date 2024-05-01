namespace Aspirate.Commands.Contracts;

public interface IBuildOptions
{
    string? RuntimeIdentifier { get; set; }
    List<string>? ComposeBuilds { get; set; }
}
