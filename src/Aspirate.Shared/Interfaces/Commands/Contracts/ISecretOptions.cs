namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface ISecretOptions
{
    SecretProviderType SecretProvider { get; set; }
}
