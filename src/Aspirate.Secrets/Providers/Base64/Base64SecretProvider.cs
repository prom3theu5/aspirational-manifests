namespace Aspirate.Secrets.Providers.Base64;

public sealed class Base64SecretProvider(IFileSystem fileSystem) : BaseSecretProvider<Base64SecretState>(fileSystem)
{
    public override string Type => AspirateSecretLiterals.Base64SecretsManager;

    public override Base64SecretState? State { get; protected set; }

    public override IEncrypter Encrypter => new Base64Encrypter();

    public override IDecrypter Decrypter => new Base64Decrypter();
}
