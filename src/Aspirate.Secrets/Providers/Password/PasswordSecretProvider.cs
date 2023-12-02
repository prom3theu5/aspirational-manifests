namespace Aspirate.Secrets.Providers.Password;

public class PasswordSecretProvider : BaseSecretProvider
{
    private const int TagSizeInBytes = 16;
    private string? _passphrase;
    private IEncrypter? _encrypter;
    private IDecrypter? _decrypter;

    public override string Type => AspirateSecretLiterals.PasswordSecretsManager;

    public override IEncrypter? Encrypter => _encrypter;

    public override IDecrypter? Decrypter => _decrypter;

    public void SetPassphrase(string passphrase)
    {
        _passphrase = passphrase;

        // Derive a key from the passphrase using Pbkdf2 with SHA256, 1 million iterations.
        using var pbkdf2 = new Rfc2898DeriveBytes(_passphrase, salt: new byte[16], iterations: 1000000, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(32); // AES-256-GCM needs a 32-byte key

        _encrypter = new AesGcmEncrypter(key, TagSizeInBytes);
        _decrypter = new AesGcmDecrypter(key, TagSizeInBytes);
    }
}
