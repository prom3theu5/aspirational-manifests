namespace Aspirate.Shared.Models.Kubernetes;

public class ImagePullSecret : Secret
{
    private ImagePullSecret()
    {
        Type = "kubernetes.io/dockerconfigjson";
        ApiVersion = "v1";
        Kind = "Secret";
    }

    public static ImagePullSecret Create() => new();

    public ImagePullSecret WithName(string name)
    {
        Metadata = new()
        {
            Name = name,
        };

        return this;
    }

    public ImagePullSecret WithDockerConfigJson(DockerConfigJson dockerConfigJson)
    {
        Data = new DockerAuthSecretData
        {
            DockerConfigJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dockerConfigJson))),
        };

        return this;
    }
}
