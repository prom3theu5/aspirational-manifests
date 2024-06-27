namespace Aspirate.Commands.Commands.Build;

public sealed class BuildCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<BuildOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(BuildOptions options) =>
        ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(PopulateInputsAction))
            .QueueAction(nameof(SubstituteValuesAspireManifestAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
            .ExecuteCommandsAsync();
}
