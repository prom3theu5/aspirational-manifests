namespace Aspirate.Commands.Actions.Manifests;

public class SubstituteValuesAspireManifestAction(IServiceProvider serviceProvider, IResourceExpressionProcessor transformer) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handle Value and Parameter Substitution[/]");

        transformer.ProcessEvaluations(CurrentState.LoadedAspireManifestResources);

        return Task.FromResult(true);
    }
}
