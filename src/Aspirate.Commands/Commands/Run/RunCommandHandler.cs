using Aspirate.Commands.Actions.KubeContext;

namespace Aspirate.Commands.Commands.Run;

public sealed class RunCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<RunOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(RunOptions options) =>
        ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(IncludeAspireDashboardAction))
            .QueueAction(nameof(PopulateInputsAction))
            .QueueAction(nameof(SubstituteValuesAspireManifestAction))
            .QueueAction(nameof(ApplyDaprAnnotationsAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
            .QueueAction(nameof(AskImagePullPolicyAction))
            .QueueAction(nameof(SaveSecretsAction))
            .QueueAction(nameof(SaveBindMountsAction))
            .QueueAction(nameof(CustomNamespaceAction))
            .QueueAction(nameof(SelectKubeContextAction))
            .QueueAction(nameof(ApplyMinikubeMountsAction))
            .QueueAction(nameof(RunKubernetesObjectsAction))
            .ExecuteCommandsAsync();
}
