namespace Aspirate.Secrets;

public class SecretProvider(IFileSystem fileSystem) : ISecretProvider
{
    private const int TagSizeInBytes = 16;
    private string? _password;
    private IEncrypter? _encrypter;
    private IDecrypter? _decrypter;
    private byte[]? _salt;
    public SecretState? State { get; set; }

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

    public void ProcessAfterStateRestoration()
    {
        if (!string.IsNullOrEmpty(_password))
        {
            _password = null;
            _decrypter = null;
            _encrypter = null;
        }

        State ??= new();

        _salt = !string.IsNullOrEmpty(State.Salt) ? Convert.FromBase64String(State.Salt) : null;
    }

    private void CreateNewSalt()
    {
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

     protected readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public void AddSecret(string resourceName, string key, string value)
    {
        if (State?.Secrets == null || _encrypter == null)
        {
            return;
        }

        var protectedValue = _encrypter?.EncryptValue(value);
        State.Secrets[resourceName][key] = protectedValue;
    }

    public void RemoveSecret(string resourceName, string key) =>
        State?.Secrets[resourceName].Remove(key);

    public bool ResourceExists(string resourceName) => State?.Secrets.TryGetValue(resourceName, out _) == true;
    public bool SecretExists(string resourceName, string key) => State?.Secrets[resourceName].TryGetValue(key, out _) == true;

    public void RemoveResource(string resourceName) =>
        State?.Secrets.Remove(resourceName);

    public void AddResource(string resourceName) =>
        State?.Secrets.Add(resourceName, []);

    public virtual void TransformStateForStorage() =>
        State.Version++;

    public void SetState(AspirateState state)
    {
        if (State == null)
        {
            return;
        }

        TransformStateForStorage();

        state.SecretState = State;
    }

    public void LoadState(AspirateState state)
    {
        State = state.SecretState;

        ProcessAfterStateRestoration();
    }

    public void RemoveState(AspirateState state)
    {
        State = null;
        state.SecretState = null;

        ProcessAfterStateRestoration();
    }

    public bool SecretStateExists(AspirateState state) => state.SecretState != null;

    public string? GetSecret(string resourceName, string key)
    {
        if (State?.Secrets == null || _decrypter == null)
        {
            return null;
        }

        return State.Secrets[resourceName].TryGetValue(key, out var encryptedValue) ? _decrypter.DecryptValue(encryptedValue) : null;
    }
}
