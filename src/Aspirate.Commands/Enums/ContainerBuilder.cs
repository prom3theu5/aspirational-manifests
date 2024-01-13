namespace Aspirate.Commands.Enums;

public class ContainerBuilder : SmartEnum<ContainerBuilder, string>
{
    private ContainerBuilder(string name, string value) : base(name, value)
    {
    }

    public static ContainerBuilder Docker = new(nameof(Docker), "docker");
    public static ContainerBuilder Podman = new(nameof(Podman), "podman");
}
