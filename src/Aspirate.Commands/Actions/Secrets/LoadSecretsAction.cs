using Aspirate.Shared.Enums;
using Aspirate.Shared.Interfaces.Secrets;

namespace Aspirate.Commands.Actions.Secrets;

public class LoadSecretsAction(
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Loading Existing Secrets[/]");

        if (CurrentState.DisableSecrets)
        {
            return Task.FromResult(true);
        }

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
                    ActionCausesExitException.ExitNow();
                }
            }
        }

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State populated successfully from [blue]{CurrentState.OutputPath}/{AspirateSecretLiterals.SecretsStateFile}[/]");

        return Task.FromResult(true);
    }

    private bool CheckPassword(PasswordSecretProvider passwordSecretProvider)
    {
        if (CliSecretPasswordSupplied(passwordSecretProvider, out var validPassword))
        {
            return validPassword;
        }

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

    private bool CliSecretPasswordSupplied(PasswordSecretProvider passwordSecretProvider, out bool validPassword)
    {
        if (string.IsNullOrEmpty(CurrentState.SecretPassword))
        {
            validPassword = false;
            return false;
        }

        if (passwordSecretProvider.CheckPassword(CurrentState.SecretPassword))
        {
            passwordSecretProvider.SetPassword(CurrentState.SecretPassword);
            {
                validPassword = true;
                return true;
            }
        }

        Logger.MarkupLine("[red]Incorrect password[/].");
        validPassword = false;
        return true;
    }

    public override void ValidateNonInteractiveState()
    {
        if (CurrentState.DisableSecrets)
        {
            return;
        }

        if (!secretProvider.SecretStateExists(CurrentState.InputPath))
        {
            return;
        }

        secretProvider.LoadState(CurrentState.InputPath);

        if (CurrentState.SecretProvider == SecretProviderType.Password && string.IsNullOrEmpty(CurrentState.SecretPassword))
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
