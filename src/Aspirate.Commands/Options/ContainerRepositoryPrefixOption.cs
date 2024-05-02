using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class ContainerRepositoryPrefixOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "--container-repository-prefix",
        "-crp",
    };

    private ContainerRepositoryPrefixOption() : base(_aliases, "ASPIRATE_CONTAINER_REPOSITORY_PREFIX", null)
    {
        Name = nameof(IContainerOptions.ContainerRepositoryPrefix);
        Description = "The Container repository prefix to use as the fall-back value for all containers";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ContainerRepositoryPrefixOption Instance { get; } = new();
}
