namespace Aspirate.Commands.Actions.Configuration;

public class AskImagePullPolicyAction(
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (PreviousStateWasRestored())
        {
            return Task.FromResult(true);
        }

        Logger.WriteRuler("[purple]Handle Image Pull Policy[/]");

        if (CurrentState.NonInteractive)
        {
            return Task.FromResult(true);
        }

        if (!string.IsNullOrEmpty(CurrentState.ImagePullPolicy))
        {
            return Task.FromResult(true);
        }

        var choices = new List<string>
        {
            "IfNotPresent",
            "Always",
            "Never",
        };

        var choice = Logger.Prompt(
            new SelectionPrompt<string>()
                .Title("Select image pull policy for manifests")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                .AddChoices(choices));

        CurrentState.ImagePullPolicy = choice;

        return Task.FromResult(true);
    }

    public override void ValidateNonInteractiveState()
    {
        if (string.IsNullOrEmpty(CurrentState.ImagePullPolicy))
        {
            NonInteractiveValidationFailed("Image pull policy is required when running in non-interactive mode.");
        }
    }
}
