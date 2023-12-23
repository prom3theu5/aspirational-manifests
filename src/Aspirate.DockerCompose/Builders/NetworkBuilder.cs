namespace Aspirate.DockerCompose.Builders;

public class NetworkBuilder : BuilderBase<NetworkBuilder, Network>
{
    internal NetworkBuilder()
    {
    }

    public NetworkBuilder SetExternal(bool isExternal) => WithProperty("external", isExternal);
}
