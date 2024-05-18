namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface ISecretState
{
    bool? ReplaceSecrets { get; set; }
}
