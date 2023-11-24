namespace Aspirate.Cli.Commands.Generate;

/// <summary>
/// The command to convert Aspire Manifests to Kustomize Manifests.
/// </summary>
public sealed class GenerateCommand(AspirateState currentState, IServiceProvider serviceProvider) : AsyncCommand<GenerateInput>
{
    public const string CommandName = "generate";
    public const string CommandDescription = "Builds, pushes containers, generates aspire manifest and kustomize manifests.";

    public override async Task<int> ExecuteAsync(CommandContext context, GenerateInput settings)
    {
        currentState.InputParameters.AspireManifestPath = settings.PathToAspireProjectFlag;
        currentState.ComputedParameters.SetKustomizeManifestPath(settings.OutputPathFlag);

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(LoadConfigurationAction.ActionKey)
            .QueueAction(GenerateAspireManifestAction.ActionKey)
            .QueueAction(LoadAspireManifestAction.ActionKey)
            .QueueAction(PopulateContainerDetailsAction.ActionKey)
            .QueueAction(BuildAndPushContainersAction.ActionKey)
            .QueueAction(GenerateKustomizeManifestAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
