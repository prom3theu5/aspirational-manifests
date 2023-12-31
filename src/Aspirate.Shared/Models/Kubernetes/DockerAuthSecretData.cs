namespace Aspirate.Shared.Models.Kubernetes;

public class DockerAuthSecretData : SecretData
{
    [YamlMember(Alias = ".dockerconfigjson")]
    public string? DockerConfigJson { get; set; }
}
