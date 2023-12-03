namespace Aspirate.Secrets.Providers.Password;

public class PasswordSecretState : BaseSecretState
{
    [JsonPropertyName("salt")]
    public string? Salt { get; set; }
}
