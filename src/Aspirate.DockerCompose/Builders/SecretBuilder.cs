namespace Aspirate.DockerCompose.Builders;

public class SecretBuilder : BuilderBase<SecretBuilder, Secret>
{
    internal SecretBuilder()
    {
    }

    public SecretBuilder WithFile(string file) => WithProperty("file", file);

    public SecretBuilder SetExternal(bool isExternal) => WithProperty("external", isExternal);
}
