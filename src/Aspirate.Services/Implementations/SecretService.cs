namespace Aspirate.Services.Implementations;

public class SecretService(
    ISecretProvider secretProvider,
    IFileSystem fs,
    IAnsiConsole logger,
    IEnumerable<ISecretProtectionStrategy> protectionStrategies)
    : ISecretService
{
    private IReadOnlyCollection<ISecretProtectionStrategy> ProtectionStrategies { get; } = protectionStrategies.ToList();

    public void SaveSecrets(SecretManagementOptions options)
    {
        if (options.DisableSecrets == true)
        {
            logger.MarkupLine("Secrets have been [red]disabled[/] for this run.");
            return;
        }

        if (!ProtectionStrategies.CheckForProtectableSecrets(options.State.AllSelectedSupportedComponents))
        {
            logger.MarkupLine("No secrets to protect in any [blue]selected components[/]");
            return;
        }

        if (secretProvider.SecretStateExists(options.State))
        {
            secretProvider.LoadState(options.State);

            if (!CheckPassword(options))
            {
                logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                ActionCausesExitException.ExitNow();
            }
        }

        if (!secretProvider.SecretStateExists(options.State))
        {
            HandleInitialisation(options);
        }

        foreach (var component in options.State.AllSelectedSupportedComponents.Where(component => !secretProvider.ResourceExists(component.Key)))
        {
            secretProvider.AddResource(component.Key);
        }

        foreach (var component in options.State.AllSelectedSupportedComponents)
        {
            if (component.Value is not IResourceWithEnvironmentalVariables { Env: not null })
            {
                continue;
            }

            foreach (var strategy in ProtectionStrategies)
            {
                strategy.ProtectSecrets(component, options.NonInteractive.GetValueOrDefault());
            }
        }

        secretProvider.SetState(options.State);

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State has been saved.");
    }

    public void ReInitialiseSecrets(SecretManagementOptions options)
    {
        if (options.DisableSecrets == true)
        {
            logger.MarkupLine("[green]Secrets are disabled[/].");
            return;
        }

        secretProvider.RemoveState(options.State);

        HandleInitialisation(options);

        secretProvider.SetState(options.State);

        options.State.SecretState = secretProvider.State;

        logger.MarkupLine("[green]Secret State has been initialised![/].");
    }

    public void LoadSecrets(SecretManagementOptions options)
    {
        logger.WriteRuler("[purple]Handling Aspirate Secrets[/]");

        if (options.DisableSecrets == true)
        {
            logger.MarkupLine("[green]Secrets are disabled[/].");
            return;
        }

        if (options.State.ReplaceSecrets == true)
        {
            ReInitialiseSecrets(options);
            return;
        }

        if (!secretProvider.SecretStateExists(options.State))
        {
            ReInitialiseSecrets(options);
            return;
        }

        secretProvider.LoadState(options.State);

        if (!options.CommandUnlocksSecrets)
        {
            logger.MarkupLine("[green]Secret State have been loaded[/], but the current command [blue]does not[/] need to decrypt them.");
            return;
        }

        if (options.NonInteractive == true)
        {
            if (string.IsNullOrEmpty(options.SecretPassword))
            {
                logger.ValidationFailed("Secrets are protected by a password, but no password has been provided.");
            }
        }

        if (!CheckPassword(options))
        {
            logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
            ActionCausesExitException.ExitNow();
        }

        options.State.SecretState = secretProvider.State;

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State populated successfully.");
    }

    private bool CheckPassword(SecretManagementOptions options)
    {
        if (CliSecretPasswordSupplied(options, out var validPassword))
        {
            return validPassword;
        }

        for (int i = 3; i > 0; i--)
        {
            var password = logger.Prompt(
                new TextPrompt<string>("Secrets are protected by a [green]password[/]. Please enter it now: ").PromptStyle("red")
                    .Secret());

            if (secretProvider.CheckPassword(password))
            {
                secretProvider.SetPassword(password);
                options.State.SecretPassword = password;
                return true;
            }

            LogPasswordError(i, "Incorrect Password");
        }

        return false;
    }

    private void LogPasswordError(int i, string caption)
    {
        var attemptsRemaining = i - 1;

        logger.MarkupLine(
            attemptsRemaining != 0
                ? $"[red]{caption}[/]. Please try again. You have [yellow]{attemptsRemaining} attempt{(attemptsRemaining > 1 ? "s" : "")}[/] remaining."
                : $"[red]{caption}[/]. You have [yellow]no attempts[/] remaining.");
    }

    private bool CliSecretPasswordSupplied(SecretManagementOptions options, out bool validPassword)
    {
        if (string.IsNullOrEmpty(options.SecretPassword))
        {
            validPassword = false;
            return false;
        }

        if (secretProvider.CheckPassword(options.SecretPassword))
        {
            secretProvider.SetPassword(options.SecretPassword);
            {
                validPassword = true;
                options.State.SecretPassword = options.SecretPassword;
                return true;
            }
        }

        logger.MarkupLine("[red]Incorrect password[/].");
        validPassword = false;
        return true;
    }

    private void HandleInitialisation(SecretManagementOptions options)
    {
        if (!CreatePassword(options))
        {
            logger.ValidationFailed("Aborting due to inability to create password.");
        }
    }

    private bool CreatePassword(SecretManagementOptions options)
    {
        logger.MarkupLine("Secrets are to be protected by a [green]password[/]");

        if (!string.IsNullOrEmpty(options.State.SecretPassword))
        {
            secretProvider.SetPassword(options.State.SecretPassword);
            return true;
        }

        for (int i = 3; i > 0; i--)
        {
            logger.WriteLine();
            var firstEntry = logger.Prompt(
                new TextPrompt<string>("Please enter new Password: ")
                    .PromptStyle("red")
                    .Secret());

            var secondEntry = logger.Prompt(
                new TextPrompt<string>("Please enter it again to confirm: ")
                    .PromptStyle("red")
                    .Secret());

            if (firstEntry.Equals(secondEntry, StringComparison.Ordinal))
            {
                secretProvider.SetPassword(firstEntry);
                options.SecretPassword = firstEntry;
                options.State.SecretPassword = firstEntry;
                return true;
            }

            LogPasswordError(i, "Passwords do not match.");
        }

        return false;
    }
}
