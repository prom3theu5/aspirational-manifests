namespace Aspirate.Secrets.Providers.Password;

public interface IPasswordSecretState
{
    string? Salt { get; set; }

    Dictionary<string, Dictionary<string, string>> Secrets { get; set; }

    int? Version { get; set; }

    string? Hash { get; set; }
}
