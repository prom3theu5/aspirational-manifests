namespace Aspirate.Secrets.Providers.Password;

public class PasswordSecretProvider(IFileSystem fileSystem) : BaseSecretProvider<PasswordSecretState>(fileSystem)
{
    private const int TagSizeInBytes = 16;
    private string? _password;
    private IEncrypter? _encrypter;
    private IDecrypter? _decrypter;
    private byte[]? _salt;

    public override PasswordSecretState? State { get; protected set; }

    public override ProviderType Type => ProviderType.Password;

    public override IEncrypter? Encrypter => _encrypter;

    public override IDecrypter? Decrypter => _decrypter;

    public void SetPassword(string password)
    {
        _password = password;

        if (_salt is null)
        {
            CreateNewSalt();
        }

        // Derive a key from the passphrase using Pbkdf2 with SHA256, 1 million iterations.
        using var pbkdf2 = new Rfc2898DeriveBytes(_password, salt: _salt, iterations: 1000000, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(32); // AES-256-GCM needs a 32-byte key
        var crypter = new AesGcmCrypter(key, _salt, TagSizeInBytes);

        _encrypter = crypter;
        _decrypter = crypter;

        SetPasswordHash();
    }

    public bool CheckPassword(string password)
    {
        using var pbkdf2ToCheck = new Rfc2898DeriveBytes(password, salt: _salt, iterations: 1000000, HashAlgorithmName.SHA256);
        var passwordToCheckHash = Convert.ToBase64String(pbkdf2ToCheck.GetBytes(32));

        return passwordToCheckHash == State.Hash;
    }

    public override void ProcessAfterStateRestoration()
    {
        if (!string.IsNullOrEmpty(_password))
        {
            _password = null;
            _decrypter = null;
            _encrypter = null;
        }

        State ??= new();

        if (!string.IsNullOrEmpty(State.Salt))
        {
            _salt = Convert.FromBase64String(State.Salt);
        }
    }

    private void CreateNewSalt()
    {
        // in .net 8 - this is 96 bit (12 bytes) max it seems
        // maybe because NIST recommends 96 bit IV for GCM to promote interoperability, efficiency, and simplicity of design on
        // page 15 of https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-38d.pdf ??
        _salt = new byte[12];
        RandomNumberGenerator.Fill(_salt);
        State ??= new();
        State.Salt = Convert.ToBase64String(_salt);
    }

    private void SetPasswordHash()
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(_password, salt: _salt, iterations: 1000000, HashAlgorithmName.SHA256);
        State.Hash = Convert.ToBase64String(pbkdf2.GetBytes(32));
    }
}
