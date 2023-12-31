namespace Aspirate.Commands.Actions.Configuration;

public class AskPrivateRegistryCredentialsAction(
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (CurrentState.NonInteractive)
        {
            return Task.FromResult(true);
        }

        if (!CurrentState.WithPrivateRegistry.GetValueOrDefault())
        {
            return Task.FromResult(true);
        }

        Logger.MarkupLine("\r\nEnsuring private registry credentials are set so that we can produce an image pull secret.");

        if (string.IsNullOrEmpty(CurrentState.RegistryUsername))
        {
            CurrentState.RegistryUsername = Logger.Prompt(
                new TextPrompt<string>("Enter registry username:")
                    .PromptStyle("blue")
                    .Validate(username => !string.IsNullOrEmpty(username), "Username is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.RegistryPassword))
        {
            CurrentState.RegistryPassword = Logger.Prompt(
                new TextPrompt<string>("Enter registry password:")
                    .Secret()
                    .PromptStyle("red")
                    .Validate(password => !string.IsNullOrEmpty(password), "Password is required and cannot be empty."));
        }

        if (string.IsNullOrEmpty(CurrentState.RegistryEmail))
        {
            CurrentState.RegistryEmail = Logger.Prompt(
                new TextPrompt<string>("Enter registry email:")
                    .PromptStyle("blue")
                    .Validate(email => !string.IsNullOrEmpty(email), "Email is required and cannot be empty."));
        }

        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Setting private registry credentials for image pull secret.");

        return Task.FromResult(true);
    }

    public override void ValidateNonInteractiveState()
    {
        if (!CurrentState.WithPrivateRegistry.GetValueOrDefault())
        {
            return;
        }

        if (string.IsNullOrEmpty(CurrentState.RegistryUsername))
        {
            NonInteractiveValidationFailed("Registry username is required when running in non-interactive mode.");
        }

        if (string.IsNullOrEmpty(CurrentState.RegistryPassword))
        {
            NonInteractiveValidationFailed("Registry password is required when running in non-interactive mode.");
        }

        if (string.IsNullOrEmpty(CurrentState.RegistryEmail))
        {
            NonInteractiveValidationFailed("Registry email is required when running in non-interactive mode.");
        }
    }
}
