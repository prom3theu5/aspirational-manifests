namespace Aspirate.Cli.Commands.Destroy;

public sealed class DestroyCommand : BaseCommand<DestroyOptions, DestroyCommandHandler>
{
    public DestroyCommand() : base("destroy", "Removes the manifests from your cluster..") =>
        AddOption(new Option<string>(new[] { "-i", "--input-path" })
        {
            Description = "The input path for the kustomize manifests to remove from the cluster",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
}

public sealed class DestroyCommandHandler(AspirateState currentState, IServiceProvider serviceProvider) : ICommandOptionsHandler<DestroyOptions>
{
    public async Task<int> HandleAsync(DestroyOptions options, CancellationToken cancellationToken)
    {
        currentState.ComputedParameters.SetKustomizeManifestPath(options.InputPath);

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(RemoveManifestsFromClusterAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
