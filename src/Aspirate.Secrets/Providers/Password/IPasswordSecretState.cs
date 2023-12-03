namespace Aspirate.Secrets.Providers.Password;

public interface IPasswordSecretState
{
    string? Salt { get; set; }

    Dictionary<string, string> Secrets { get; set; }

    int? Version { get; set; }
}
