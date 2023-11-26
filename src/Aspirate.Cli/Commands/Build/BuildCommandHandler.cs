namespace Aspirate.Cli.Commands.Build;

public sealed class BuildCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<BuildOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(BuildOptions options) =>
        ActionExecutor
            .QueueAction(LoadConfigurationAction.ActionKey)
            .QueueAction(GenerateAspireManifestAction.ActionKey)
            .QueueAction(LoadAspireManifestAction.ActionKey)
            .QueueAction(PopulateContainerDetailsAction.ActionKey)
            .QueueAction(BuildAndPushContainersAction.ActionKey)
            .ExecuteCommandsAsync();
}
