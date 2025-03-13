namespace Aspirate.Commands.Options;

public sealed class ContainerBuildContextOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    [
        "-cbc",
        "--container-build-context"
    ];

    private ContainerBuildContextOption() : base(_aliases, "ASPIRATE_CONTAINER_BUILD_CONTEXT", null)
    {
        Name = nameof(IContainerOptions.ContainerBuildContext);
        Description = "The Container Build Context to use when Dockerfile is used to build projects";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ContainerBuildContextOption Instance { get; } = new();

    public override bool IsSecret => false;
}
