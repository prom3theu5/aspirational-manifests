namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptions options) =>
        ActionExecutor
            .QueueAction(LoadConfigurationAction.ActionKey)
            .QueueAction(GenerateAspireManifestAction.ActionKey)
            .QueueAction(LoadAspireManifestAction.ActionKey)
            .QueueAction(PopulateContainerDetailsAction.ActionKey)
            .QueueAction(BuildAndPushContainersAction.ActionKey)
            .QueueAction(GenerateKustomizeManifestAction.ActionKey)
            .ExecuteCommandsAsync();
}
