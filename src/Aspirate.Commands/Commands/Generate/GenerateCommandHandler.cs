using Aspirate.Commands.Actions.BindMounts;

namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommandHandler(IServiceProvider serviceProvider) : BaseCommandOptionsHandler<GenerateOptions>(serviceProvider)
{
    public override Task<int> HandleAsync(GenerateOptions options)
    {
        if (!OutputFormat.TryFromValue(CurrentState.OutputFormat, out var outputFormat))
        {
            throw new ArgumentOutOfRangeException(nameof(CurrentState.OutputFormat), $"The output format '{CurrentState.OutputFormat}' is not supported.");
        }

        return outputFormat.Name switch
        {
            nameof(OutputFormat.Kustomize) => GenerateKustomizeManifests(),
            nameof(OutputFormat.DockerCompose) => GenerateDockerComposeManifests(),
            nameof(OutputFormat.Helm) => GenerateHelmManifests(),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OutputFormat), $"The output format '{options.OutputFormat}' is not supported."),
        };
    }

    private ActionExecutor BaseGenerateActionSequence() =>
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
            .QueueAction(nameof(SaveSecretsAction))
            .QueueAction(nameof(SaveBindMountsAction));

    private ActionExecutor BaseKubernetesActionSequence() =>
        BaseGenerateActionSequence()
            .QueueAction(nameof(AskImagePullPolicyAction));

    private Task<int> GenerateDockerComposeManifests() =>
        BaseGenerateActionSequence()
            .QueueAction(nameof(GenerateDockerComposeManifestAction))
            .ExecuteCommandsAsync();

    private Task<int> GenerateHelmManifests() =>
        BaseKubernetesActionSequence()
            .QueueAction(nameof(GenerateHelmChartAction))
            .ExecuteCommandsAsync();

    private Task<int> GenerateKustomizeManifests() =>
        BaseKubernetesActionSequence()
            .QueueAction(nameof(CustomNamespaceAction))
            .QueueAction(nameof(GenerateKustomizeManifestsAction))
            .QueueAction(nameof(GenerateFinalKustomizeManifestAction))
            .ExecuteCommandsAsync();
}
