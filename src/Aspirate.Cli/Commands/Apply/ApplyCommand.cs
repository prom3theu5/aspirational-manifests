namespace Aspirate.Cli.Commands.Apply;

public sealed class ApplyCommand : BaseCommand<ApplyOptions, ApplyCommandHandler>
{
    public ApplyCommand() : base("apply", "Apply the generated kustomize manifest to the cluster.") =>
        AddOption(new Option<string>(new[] { "-o", "--output-path" })
        {
            Description = "The input path for the generated kustomize manifests",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
}

public sealed class ApplyCommandHandler(AspirateState currentState, IServiceProvider serviceProvider) : ICommandOptionsHandler<ApplyOptions>
{
    public async Task<int> HandleAsync(ApplyOptions options, CancellationToken cancellationToken)
    {
        currentState.ComputedParameters.SetKustomizeManifestPath(options.OutputPath);

        var actionExecutor = ActionExecutor.CreateInstance(serviceProvider);

        await actionExecutor
            .QueueAction(ApplyManifestsToClusterAction.ActionKey)
            .ExecuteCommandsAsync();

        return 0;
    }
}
