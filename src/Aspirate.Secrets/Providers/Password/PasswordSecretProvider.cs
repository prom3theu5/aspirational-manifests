namespace Aspirate.Secrets.Providers.Password;

public class PasswordSecretProvider(IFileSystem fileSystem) : BaseSecretProvider<PasswordSecretState>(fileSystem)
{
    private const int TagSizeInBytes = 16;
    private string? _password;
    private string? _salt;
    private IEncrypter? _encrypter;
    private IDecrypter? _decrypter;

    public override PasswordSecretState? State { get; protected set; }

    public override string Type => AspirateSecretLiterals.PasswordSecretsManager;

    public override IEncrypter? Encrypter => _encrypter;

    public override IDecrypter? Decrypter => _decrypter;

    public void SetPassword(string password)
    {
        _password = password;

        if (string.IsNullOrEmpty(_salt))
        {
            CreateNewSalt();
        }

        // Derive a key from the passphrase using Pbkdf2 with SHA256, 1 million iterations.
        var saltBytes = Convert.FromBase64String(_salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(_password, salt: saltBytes, iterations: 1000000, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(32); // AES-256-GCM needs a 32-byte key

        _encrypter = new AesGcmEncrypter(key, TagSizeInBytes);
        _decrypter = new AesGcmDecrypter(key, TagSizeInBytes);
    }

    private void CreateNewSalt()
    {
        var newSaltBytes = new byte[16];
        RandomNumberGenerator.Fill(newSaltBytes);
        _salt = Convert.ToBase64String(newSaltBytes);
    }
}
