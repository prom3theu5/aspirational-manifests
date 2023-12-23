namespace Aspirate.DockerCompose.Builders;

public class VolumeBuilder : BuilderBase<VolumeBuilder, Volume>
{
    internal VolumeBuilder()
    {
    }

    public VolumeBuilder SetExternal(bool isExternal) => WithProperty("external", isExternal);
}
