namespace Aspirate.Commands.Contracts;

public interface IAspireOptions
{
    string? ProjectPath { get; set; }
    string? AspireManifest { get; set; }
}
