namespace Aspirate.Secrets.Providers.Password;

public class PasswordSecretState : BaseSecretState, IPasswordSecretState
{
    [JsonPropertyName("salt")]
    public string? Salt { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
}
