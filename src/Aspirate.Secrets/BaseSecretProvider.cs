namespace Aspirate.Secrets;

public abstract class BaseSecretProvider<TState>(IFileSystem fileSystem) : ISecretProvider where TState : BaseSecretState, new()
{
    protected readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public abstract string Type { get; }

    public abstract TState? State { get; protected set; }

    public abstract IEncrypter? Encrypter { get; }

    public abstract IDecrypter? Decrypter { get; }

    public void AddSecret(string key, string value)
    {
        if (State?.Secrets == null || Encrypter == null)
        {
            return;
        }

        State.Secrets[key] = Encrypter?.EncryptValue(value);
    }

    public void RemoveSecret(string key) =>
        State?.Secrets.Remove(key);

    public virtual void RestoreState(string state)
    {
        State = JsonSerializer.Deserialize<TState>(state, _serializerOptions);
        ProcessAfterStateRestoration();
    }

    public virtual void TransformStateForStorage() =>
        State.Version++;

    public virtual void ProcessAfterStateRestoration()
    {}

    public void SaveState(string? path = null)
    {
        if (State == null)
        {
            return;
        }

        TransformStateForStorage();

        path ??= fileSystem.Directory.GetCurrentDirectory();

        var state = JsonSerializer.Serialize(State, _serializerOptions);
        var outputFile = fileSystem.Path.Combine(path, AspirateSecretLiterals.SecretsStateFile);
        fileSystem.File.WriteAllText(outputFile, state);
    }

    public string? GetSecret(string key)
    {
        if (State?.Secrets == null || Decrypter == null)
        {
            return null;
        }

        return State.Secrets.TryGetValue(key, out var encryptedValue) ? Decrypter.DecryptValue(encryptedValue) : null;
    }
}
