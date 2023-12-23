namespace Aspirate.Commands.Actions.Manifests;

public class GenerateDockerComposeManifestAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        var outputFormat = OutputFormat.FromValue(CurrentState.OutputFormat);

        if (outputFormat == OutputFormat.Kustomize)
        {
            Logger.MarkupLine($"[red](!)[/] The output format '{CurrentState.OutputFormat}' is not supported for this action.");
            Logger.MarkupLine($"[red](!)[/] Please use the output format 'compose' instead.");
            throw new ActionCausesExitException(1);
        }

        return Task.FromResult(true);
    }
}
