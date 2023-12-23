namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptions options)
    {
        if (!OutputFormat.TryFromValue(options.OutputFormat, out var outputFormat))
        {
            throw new ArgumentOutOfRangeException(nameof(options.OutputFormat), $"The output format '{options.OutputFormat}' is not supported.");
        }

        return outputFormat.Name switch
        {
            nameof(OutputFormat.Kustomize) => GenerateKustomizeManifests(),
            nameof(OutputFormat.DockerCompose) => GenerateDockerComposeManifests(),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OutputFormat), $"The output format '{options.OutputFormat}' is not supported."),
        };
    }

    private Task<int> GenerateDockerComposeManifests() =>
        ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(PopulateInputsAction))
            .QueueAction(nameof(SubstituteValuesAspireManifestAction))
            .QueueAction(nameof(ApplyDaprAnnotationsAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
            .QueueAction(nameof(GenerateDockerComposeManifestAction))
            .ExecuteCommandsAsync();

    private Task<int> GenerateKustomizeManifests() =>
        ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(PopulateInputsAction))
            .QueueAction(nameof(SubstituteValuesAspireManifestAction))
            .QueueAction(nameof(ApplyDaprAnnotationsAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction))
            .QueueAction(nameof(AskImagePullPolicyAction))
            .QueueAction(nameof(SaveSecretsAction))
            .QueueAction(nameof(GenerateKustomizeManifestsAction))
            .QueueAction(nameof(GenerateFinalKustomizeManifestAction))
            .ExecuteCommandsAsync();
}
