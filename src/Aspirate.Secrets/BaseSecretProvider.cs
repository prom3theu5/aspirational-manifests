namespace Aspirate.Secrets;

public abstract class BaseSecretProvider : ISecretProvider
{
    public abstract string Type { get; }

    public string? State { get; protected set; }

    public abstract IEncrypter? Encrypter { get; }

    public abstract IDecrypter? Decrypter { get; }

    public virtual void RestoreState(string state) =>
        State = state;
}
