namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyCommand : BaseCommand<ApplyOptions, ApplyCommandHandler>
{
    protected override bool CommandUnlocksSecrets => true;

    public ApplyCommand() : base("apply", "Apply the generated kustomize manifest to the cluster.")
    {
        AddOption(InputPathOption.Instance);
        AddOption(KubernetesContextOption.Instance);
        AddOption(SecretPasswordOption.Instance);
        AddOption(RollingRestartOption.Instance);
        AddOption(EnableMinikubeMountActionOption.Instance);
    }
}
