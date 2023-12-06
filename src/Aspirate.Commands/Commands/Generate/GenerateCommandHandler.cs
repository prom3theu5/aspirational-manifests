namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptions options) =>
        ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
            .QueueAction(nameof(AskImagePullPolicyAction))
            .QueueAction(nameof(GenerateKustomizeManifestsAction))
            .QueueAction(nameof(GenerateFinalKustomizeManifestAction))
            .ExecuteCommandsAsync();
}
