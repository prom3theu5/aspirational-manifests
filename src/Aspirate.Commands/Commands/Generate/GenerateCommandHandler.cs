namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptions options)
    {
        var outputFormatString = !string.IsNullOrEmpty(CurrentState.OutputFormat) ?
            CurrentState.OutputFormat : !string.IsNullOrEmpty(options.OutputFormat) ?
                options.OutputFormat : OutputFormat.Kustomize.Value;

        if (!OutputFormat.TryFromValue(outputFormatString, out var outputFormat))
        {
            throw new ArgumentOutOfRangeException(nameof(outputFormatString), $"The output format '{outputFormatString}' is not supported.");
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
            .QueueAction(nameof(IncludeAspireDashboardAction))
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
            .QueueAction(nameof(IncludeAspireDashboardAction))
            .QueueAction(nameof(AskPrivateRegistryCredentialsAction))
            .QueueAction(nameof(CustomNamespaceAction))
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
            .QueueAction(nameof(GenerateHelmChartAction))
            .ExecuteCommandsAsync();
}
