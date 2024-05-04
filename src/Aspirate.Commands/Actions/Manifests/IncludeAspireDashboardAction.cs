namespace Aspirate.Commands.Actions.Manifests;

public class IncludeAspireDashboardAction(IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handling Aspire Dashboard[/]");

        if (CurrentState.IncludeDashboard != null)
        {
            return Task.FromResult(true);
        }

        if (CurrentState.NonInteractive)
        {
            Logger.ValidationFailed("The include dashboard option is required in non-interactive mode.");
        }

        AskShouldIncludeDashboard();

        return Task.FromResult(true);
    }

    private void AskShouldIncludeDashboard()
    {
        var shouldIncludeAspireDashboard = Logger.Confirm(
            "[bold]Would you like to deploy the aspire dashboard and connect the OTLP endpoint?[/]");

        if (!shouldIncludeAspireDashboard)
        {
            Logger.MarkupLine("[yellow](!)[/] Skipping Aspire Dashboard deployment");
            CurrentState.IncludeDashboard = false;
            return;
        }

        CurrentState.IncludeDashboard = true;
    }

    public override void ValidateNonInteractiveState()
    {
        if (CurrentState.IncludeDashboard == null)
        {
            Logger.ValidationFailed("The include dashboard option is required in non-interactive mode.");
        }
    }
}
