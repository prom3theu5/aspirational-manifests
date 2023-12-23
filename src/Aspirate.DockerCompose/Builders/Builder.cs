namespace Aspirate.DockerCompose.Builders;

public class Builder
{
    public static ServiceBuilder MakeService() => new();

    public static ServiceBuilder MakeService(string serviceName) => new ServiceBuilder().WithName(serviceName);

    public static ComposeBuilder MakeCompose() => new ComposeBuilder().WithVersion("3.8");

    public static VolumeBuilder MakeVolume() => new();

    public static VolumeBuilder MakeVolume(string name) => new VolumeBuilder().WithName(name);

    public static SecretBuilder MakeSecret() => new();

    public static SecretBuilder MakeSecret(string name) => new SecretBuilder().WithName(name);


    public static NetworkBuilder MakeNetwork() => new();

    public static NetworkBuilder MakeNetwork(string name) => new NetworkBuilder().WithName(name);

    public static MapBuilder MakeMap() => new();

    public static MapBuilder MakeMap(string name) => new MapBuilder().WithName(name);

    public static DeployBuilder MakeDeploy() => new();
}
