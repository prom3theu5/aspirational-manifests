namespace Aspirate.Commands.Options;

public sealed class ContainerImageTagOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "-ct",
        "--container-image-tag"
    };

    private ContainerImageTagOption() : base(_aliases, "ASPIRATE_CONTAINER_IMAGE_TAG", null)
    {
        Name = nameof(BuildOptions.ContainerImageTag);
        Description = "The Container Image Tag to use as the fall-back value for all containers";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ContainerImageTagOption Instance { get; } = new();
}
