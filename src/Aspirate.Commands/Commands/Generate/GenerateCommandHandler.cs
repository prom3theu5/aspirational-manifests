using Aspirate.Shared.Enums;

namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptionses>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptionses optionses)
    {
        if (!OutputFormat.TryFromValue(optionses.OutputFormat, out var outputFormat))
        {
            throw new ArgumentOutOfRangeException(nameof(optionses.OutputFormat), $"The output format '{optionses.OutputFormat}' is not supported.");
        }

        if (outputFormat.Name == nameof(OutputFormat.DockerCompose))
        {
            CurrentState.DisableSecrets = true;
        }

        return outputFormat.Name switch
        {
            nameof(OutputFormat.Kustomize) => GenerateKustomizeManifests(),
            nameof(OutputFormat.DockerCompose) => GenerateDockerComposeManifests(),
            _ => throw new ArgumentOutOfRangeException(nameof(optionses.OutputFormat), $"The output format '{optionses.OutputFormat}' is not supported."),
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
