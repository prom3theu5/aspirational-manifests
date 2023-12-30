namespace Aspirate.Shared.Models.Kubernetes;

public class Secret
{
    public string? ApiVersion { get; set; }
    public string? Kind { get; set; }
    public SecretMetadata? Metadata { get; set; }
    public string? Type { get; set; }
    public SecretData? Data { get; set; }
}
