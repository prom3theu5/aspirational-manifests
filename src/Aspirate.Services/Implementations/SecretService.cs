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
            logger.MarkupLine("Aspirate Secrets [blue]already exist[/] for manifest.");

            secretProvider.LoadState(options.State);

            if (!CheckPassword(options))
            {
                logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                ActionCausesExitException.ExitNow();
            }

            if (!options.NonInteractive == true)
            {
                if (options.State.ExistingSecretsType is not null)
                {
                    if (HandleSecretAction(options, options.State.ExistingSecretsType.Name))
                    {
                        return;
                    }
                }
                else
                {
                    var secretsAction = logger.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select the action for the existing secrets...")
                            .HighlightStyle("blue")
                            .PageSize(3)
                            .AddChoices(ExistingSecretsType.Existing.Name, ExistingSecretsType.Augment.Name, ExistingSecretsType.Overwrite.Name));

                    if (HandleSecretAction(options, secretsAction))
                    {
                        return;
                    }
                }
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

    private bool HandleSecretAction(SecretManagementOptions options, string secretsAction)
    {
        if (secretsAction.Equals(ExistingSecretsType.Existing.Name, StringComparison.Ordinal))
        {
            logger.MarkupLine("Using [green]existing[/] secrets.");
            options.State.ExistingSecretsType = ExistingSecretsType.Existing;
            return true;
        }

        if (secretsAction.Equals(ExistingSecretsType.Augment.Name, StringComparison.Ordinal))
        {
            options.State.ExistingSecretsType = ExistingSecretsType.Augment;
            logger.MarkupLine("Using [green]existing[/] secrets and augmenting with new values.");
            return false;
        }

        logger.MarkupLine("[yellow]Overwriting[/] secrets");
        options.State.ExistingSecretsType = ExistingSecretsType.Overwrite;
        secretProvider.RemoveState(options.State);
        return false;
    }

    public void LoadSecrets(SecretManagementOptions options)
    {
        logger.WriteRuler("[purple]Handling Aspirate Secrets[/]");

        if (options.DisableSecrets == true)
        {
            logger.MarkupLine("[green]Secrets are disabled[/].");
            return;
        }

        if (!secretProvider.SecretStateExists(options.State))
        {
            logger.MarkupLine("[green]No existing secrets state found[/].");
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

        for (int i = 0; i < 3; i++)
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

            logger.MarkupLine($"[red]Incorrect password[/]. Please try again. You have [yellow]{3 - i} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
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
                return true;
            }

            logger.MarkupLine($"[red]Passwords do not match[/]. Please try again. You have [yellow]{i - 1} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }
}
