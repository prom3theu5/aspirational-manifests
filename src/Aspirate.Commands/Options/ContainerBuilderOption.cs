namespace Aspirate.Commands.Options;

public sealed class ContainerBuilderOption : BaseOption<string>
{
    private static readonly string[] _aliases = { "--container-builder" };

    private ContainerBuilderOption() : base(_aliases, "ASPIRATE_CONTAINER_BUILDER", "docker")
    {
        Name = nameof(IContainerOptions.ContainerBuilder);
        Description = "The Container Builder: can be 'docker' or 'podman'. The default is 'docker'";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ContainerBuilderOption Instance { get; } = new();
}
