namespace Aspirate.Commands.Actions.Secrets;

public class LoadSecretsAction(
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (!secretProvider.SecretStateExists(CurrentState.InputPath))
        {
            return Task.FromResult(true);
        }

        if (!CurrentState.NonInteractive)
        {
            secretProvider.LoadState(CurrentState.InputPath);

            if (secretProvider is PasswordSecretProvider passwordSecretProvider)
            {
                if (!CheckPassword(passwordSecretProvider))
                {
                    Logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                    throw new ActionCausesExitException(1);
                }
            }
        }

        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State populated successfully from [blue]{CurrentState.OutputPath}/{AspirateSecretLiterals.SecretsStateFile}[/]");

        return Task.FromResult(true);
    }

    private bool CheckPassword(PasswordSecretProvider passwordSecretProvider)
    {
        for (int i = 0; i < 3; i++)
        {
            var password = Logger.Prompt(
                new TextPrompt<string>("Secrets are protected by a [green]password[/]. Please enter it now: ").PromptStyle("red")
                    .Secret());

            if (passwordSecretProvider.CheckPassword(password))
            {
                passwordSecretProvider.SetPassword(password);
                return true;
            }

            Logger.MarkupLine($"[red]Incorrect password[/]. Please try again. You have [yellow]{3 - i} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }

    public override void ValidateNonInteractiveState()
    {
        if (!secretProvider.SecretStateExists(CurrentState.InputPath))
        {
            return;
        }

        secretProvider.LoadState(CurrentState.InputPath);

        if (CurrentState.SecretProvider == ProviderType.Password && string.IsNullOrEmpty(CurrentState.SecretPassword))
        {
            NonInteractiveValidationFailed("Secrets are protected by a password, but no password has been provided.");
        }

        if (secretProvider is not PasswordSecretProvider passwordSecretProvider)
        {
            return;
        }

        if (!passwordSecretProvider.CheckPassword(CurrentState.SecretPassword))
        {
            NonInteractiveValidationFailed("Secrets are protected by a password, but the provided password is incorrect.");
        }

        passwordSecretProvider.SetPassword(CurrentState.SecretPassword);
    }
}
