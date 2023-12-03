namespace Aspirate.Secrets.Providers.Base64;

public interface IBase64SecretState
{
    Dictionary<string, string> Secrets { get; set; }

    int? Version { get; set; }
}
