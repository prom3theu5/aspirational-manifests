using Aspirate.Shared.Interfaces.Commands.Contracts;

namespace Aspirate.Commands.Options;

public sealed class KubernetesContextOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    {
        "-k",
        "--kube-context"
    };

    private KubernetesContextOption() : base(_aliases, "ASPIRATE_KUBERNETES_CONTEXT", null)
    {
        Name = nameof(IKubernetesOptions.KubeContext);
        Description = "The name of the kubernetes context to use";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static KubernetesContextOption Instance { get; } = new();
}
