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
            nameof(OutputFormat.DockerCompose) => GenerateDockerComposeManifests(options.UseEnvVariablesAsParameterValues ?? false),
            nameof(OutputFormat.Helm) => GenerateHelmManifests(),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OutputFormat), $"The output format '{options.OutputFormat}' is not supported."),
        };
    }

    private ActionExecutor BaseGenerateActionSequence(bool useEnvVariablesAsParameterValues = false)
    {
        var result = ActionExecutor
            .QueueAction(nameof(LoadConfigurationAction))
            .QueueAction(nameof(GenerateAspireManifestAction))
            .QueueAction(nameof(LoadAspireManifestAction))
            .QueueAction(nameof(IncludeAspireDashboardAction));
        if (!useEnvVariablesAsParameterValues)
        {
            result.QueueAction(nameof(PopulateInputsAction));
        }
        else
        {
            result.QueueAction(nameof(PopulateInputsWithEnvVariablesAction));
        }
        result
            .QueueAction(nameof(SubstituteValuesAspireManifestAction))
            .QueueAction(nameof(ApplyDaprAnnotationsAction))
            .QueueAction(nameof(PopulateContainerDetailsForProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromProjectsAction))
            .QueueAction(nameof(BuildAndPushContainersFromDockerfilesAction));
        if (!useEnvVariablesAsParameterValues)
        {
            result.QueueAction(nameof(SaveSecretsAction));
        }

        return result;
    }

    private ActionExecutor BaseKubernetesActionSequence() =>
        BaseGenerateActionSequence()
            .QueueAction(nameof(AskImagePullPolicyAction));

    private Task<int> GenerateDockerComposeManifests(bool useEnvVariablesAsParameterValues) =>
        BaseGenerateActionSequence(useEnvVariablesAsParameterValues)
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
