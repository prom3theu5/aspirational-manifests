namespace Aspirate.Shared.Interfaces.Secrets;

public interface IBase64SecretState
{
    Dictionary<string, Dictionary<string, string>> Secrets { get; set; }

    int? Version { get; set; }
}
