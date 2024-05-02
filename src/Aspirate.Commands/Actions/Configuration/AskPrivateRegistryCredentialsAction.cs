using Aspirate.Shared.Interfaces.Services;

namespace Aspirate.Commands.Actions.Configuration;

public class AskPrivateRegistryCredentialsAction(
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling private registry credentials[/]");

        if (CurrentState.NonInteractive)
        {
            return Task.FromResult(true);
        }

        if (!CurrentState.WithPrivateRegistry.GetValueOrDefault())
        {
            return Task.FromResult(true);
        }

        Logger.MarkupLine("Ensuring private registry credentials are set so that we can produce an image pull secret.");

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUrl))
        {
            CurrentState.PrivateRegistryUrl = Logger.Prompt(
                new TextPrompt<string>("Enter registry url:")
                    .PromptStyle("blue")
                    .Validate(url => !string.IsNullOrEmpty(url), "Url required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUsername))
        {
            CurrentState.PrivateRegistryUsername = Logger.Prompt(
                new TextPrompt<string>("Enter registry username:")
                    .PromptStyle("blue")
                    .Validate(username => !string.IsNullOrEmpty(username), "Username is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryPassword))
        {
            CurrentState.PrivateRegistryPassword = Logger.Prompt(
                new TextPrompt<string>("Enter registry password:")
                    .Secret()
                    .PromptStyle("red")
                    .Validate(password => !string.IsNullOrEmpty(password), "Password is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryEmail))
        {
            CurrentState.PrivateRegistryEmail = Logger.Prompt(
                new TextPrompt<string>("Enter registry email:")
                    .PromptStyle("blue")
                    .Validate(email => !string.IsNullOrEmpty(email), "Email is required and cannot be empty."));
        }

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Setting private registry credentials for image pull secret.");

        return Task.FromResult(true);
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.WithPrivateRegistry.GetValueOrDefault())
        {
            return;
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUrl))
        {
            NonInteractiveValidationFailed("Registry url is required when running in non-interactive mode.");
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryUsername))
        {
            NonInteractiveValidationFailed("Registry username is required when running in non-interactive mode.");
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryPassword))
        {
            NonInteractiveValidationFailed("Registry password is required when running in non-interactive mode.");
        }

        if (string.IsNullOrEmpty(CurrentState.PrivateRegistryEmail))
        {
            NonInteractiveValidationFailed("Registry email is required when running in non-interactive mode.");
        }
    }
}
