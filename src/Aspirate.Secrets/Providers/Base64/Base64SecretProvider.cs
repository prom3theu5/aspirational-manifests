namespace Aspirate.Secrets.Providers.Base64;

public sealed class Base64SecretProvider : BaseSecretProvider
{
    public override string Type => AspirateSecretLiterals.Base64SecretsManager;

    public override IEncrypter Encrypter => new Base64Encrypter();

    public override IDecrypter Decrypter => new Base64Decrypter();
}
