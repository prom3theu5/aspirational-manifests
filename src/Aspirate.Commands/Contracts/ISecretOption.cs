namespace Aspirate.Commands.Contracts;

public interface ISecretOption
{
    ProviderType SecretProvider { get; set; }
    string? SecretPassword { get; set; }
}
