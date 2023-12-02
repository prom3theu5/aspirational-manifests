namespace Aspirate.Secrets.Services;
public interface ISecretProvider
{
    string Type { get; }
    string? State { get; }
    IEncrypter? Encrypter { get; }
    IDecrypter? Decrypter { get; }
}
