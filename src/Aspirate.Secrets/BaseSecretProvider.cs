namespace Aspirate.Secrets;

public abstract class BaseSecretProvider<TState>(IFileSystem fileSystem) : ISecretProvider where TState : BaseSecretState, new()
{
    protected readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public abstract ProviderType Type { get; }

    public abstract TState? State { get; protected set; }

    public abstract IEncrypter? Encrypter { get; }

    public abstract IDecrypter? Decrypter { get; }

    public void AddSecret(string resourceName, string key, string value)
    {
        if (State?.Secrets == null || Encrypter == null)
        {
            return;
        }

        State.Secrets[resourceName][key] = Encrypter?.EncryptValue(value);
    }

    public void RemoveSecret(string resourceName, string key) =>
        State?.Secrets[resourceName].Remove(key);

    public void RemoveResource(string resourceName) =>
        State?.Secrets.Remove(resourceName);

    public void AddResource(string resourceName) =>
        State?.Secrets.Add(resourceName, []);

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

    public void LoadState(string? path = null)
    {
        path ??= fileSystem.Directory.GetCurrentDirectory();
        var inputFile = fileSystem.Path.Combine(path, AspirateSecretLiterals.SecretsStateFile);

        if (!fileSystem.File.Exists(inputFile))
        {
            throw new FileNotFoundException($"State file not found: {inputFile}");
        }

        var stateJson = fileSystem.File.ReadAllText(inputFile);
        State = JsonSerializer.Deserialize<TState>(stateJson, _serializerOptions);

        ProcessAfterStateRestoration();
    }

    public bool SecretStateExists(string? path = null)
    {
        path ??= fileSystem.Directory.GetCurrentDirectory();
        var inputFile = fileSystem.Path.Combine(path, AspirateSecretLiterals.SecretsStateFile);

        return fileSystem.File.Exists(inputFile);
    }

    public string? GetSecret(string resourceName, string key)
    {
        if (State?.Secrets == null || Decrypter == null)
        {
            return null;
        }

        return State.Secrets[resourceName].TryGetValue(key, out var encryptedValue) ? Decrypter.DecryptValue(encryptedValue) : null;
    }
}
