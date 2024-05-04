namespace Aspirate.Services.Implementations;

public class SecretService(
    ISecretProvider secretProvider,
    IFileSystem fs,
    IAnsiConsole logger,
    IEnumerable<ISecretProtectionStrategy> protectionStrategies)
    : ISecretService
{
    private const string UseExisting = "Use Existing";
    private const string Augment = "Augment by adding / replacing values";
    private const string Overwrite = "Overwrite / Create new Password";

    private IReadOnlyCollection<ISecretProtectionStrategy> ProtectionStrategies { get; } = protectionStrategies.ToList();

    public void SaveSecrets(SecretManagementOptions options)
    {
        var secretStatePath = fs.GetSecretsStateFilePath(options.State);

        if (string.IsNullOrEmpty(secretStatePath))
        {
            logger.ValidationFailed("Secrets state file path is not valid.");
        }

        if (options.DisableSecrets)
        {
            logger.MarkupLine("Secrets have been [red]disabled[/] for this run.");
            return;
        }

        if (!ProtectionStrategies.CheckForProtectableSecrets(options.State.AllSelectedSupportedComponents))
        {
            logger.MarkupLine("No secrets to protect in any [blue]selected components[/]");
            return;
        }

        if (secretProvider.SecretStateExists(secretStatePath))
        {
            logger.MarkupLine("Aspirate Secrets [blue]already exist[/] for manifest.");

            secretProvider.LoadState(secretStatePath);

            if (secretProvider is PasswordSecretProvider passwordSecretProvider)
            {
                if (!CheckPassword(passwordSecretProvider, options))
                {
                    logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                    ActionCausesExitException.ExitNow();
                }
            }

            if (!options.NonInteractive)
            {
                var secretsAction = logger.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select the action for the existing secrets...")
                        .HighlightStyle("blue")
                        .PageSize(3)
                        .AddChoices(UseExisting, Augment, Overwrite));

                switch (secretsAction)
                {
                    case UseExisting:
                        logger.MarkupLine($"Using [green]existing[/] secrets for provider [blue]{secretProvider.Type}[/]");
                        return;
                    case Augment:
                        logger.MarkupLine(
                            $"Using [green]existing[/] secrets for provider [blue]{secretProvider.Type}[/] and augmenting with new values.");
                        break;
                    case Overwrite:
                        logger.MarkupLine($"[yellow]Overwriting[/] secrets for provider [blue]{secretProvider.Type}[/]");
                        secretProvider.RemoveState(secretStatePath);
                        break;
                }
            }
        }

        if (!secretProvider.SecretStateExists(secretStatePath))
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
                strategy.ProtectSecrets(component, options.NonInteractive);
            }
        }

        secretProvider.SaveState(secretStatePath);

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State has been saved to [blue]{secretStatePath}[/]");
    }

    public void LoadSecrets(SecretManagementOptions options)
    {
        var secretStatePath = fs.GetSecretsStateFilePath(options.State);

        if (string.IsNullOrEmpty(secretStatePath))
        {
            logger.ValidationFailed("Secrets state file path is not valid.");
        }

        logger.WriteRuler("[purple]Loading Existing Secrets[/]");

        if (options.DisableSecrets)
        {
            logger.MarkupLine("[green]Secrets are disabled[/].");
            return;
        }

        if (!secretProvider.SecretStateExists(secretStatePath))
        {
            logger.MarkupLine("[green]No existing secrets state found[/].");
            return;
        }

        if (options.NonInteractive)
        {
            if (secretProvider is PasswordSecretProvider && string.IsNullOrEmpty(options.SecretPassword))
            {
                logger.ValidationFailed("Secrets are protected by a password, but no password has been provided.");
            }
        }

        secretProvider.LoadState(secretStatePath);

        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            if (!CheckPassword(passwordSecretProvider, options))
            {
                logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                ActionCausesExitException.ExitNow();
            }

            options.State.Secrets = passwordSecretProvider.State?.Secrets;
        }

        if (secretProvider is Base64SecretProvider base64SecretProvider)
        {
            options.State.Secrets = base64SecretProvider.State?.Secrets;
        }

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State populated successfully from [blue]{secretStatePath}[/]");
    }

    private bool CheckPassword(PasswordSecretProvider passwordSecretProvider, SecretManagementOptions options)
    {
        if (CliSecretPasswordSupplied(passwordSecretProvider, options, out var validPassword))
        {
            return validPassword;
        }

        for (int i = 0; i < 3; i++)
        {
            var password = logger.Prompt(
                new TextPrompt<string>("Secrets are protected by a [green]password[/]. Please enter it now: ").PromptStyle("red")
                    .Secret());

            if (passwordSecretProvider.CheckPassword(password))
            {
                passwordSecretProvider.SetPassword(password);
                options.State.SecretPassword = password;
                return true;
            }

            logger.MarkupLine($"[red]Incorrect password[/]. Please try again. You have [yellow]{3 - i} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }

    private bool CliSecretPasswordSupplied(PasswordSecretProvider passwordSecretProvider, SecretManagementOptions options, out bool validPassword)
    {
        if (string.IsNullOrEmpty(options.SecretPassword))
        {
            validPassword = false;
            return false;
        }

        if (passwordSecretProvider.CheckPassword(options.SecretPassword))
        {
            passwordSecretProvider.SetPassword(options.SecretPassword);
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
        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            if (!CreatePassword(passwordSecretProvider, options))
            {
                logger.ValidationFailed("Aborting due to inability to create password.");
            }
        }
    }

    private bool CreatePassword(PasswordSecretProvider passwordSecretProvider, SecretManagementOptions options)
    {
        logger.MarkupLine("Secrets are to be protected by a [green]password[/]");

        if (!string.IsNullOrEmpty(options.State.SecretPassword))
        {
            passwordSecretProvider.SetPassword(options.State.SecretPassword);
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
                passwordSecretProvider.SetPassword(firstEntry);
                return true;
            }

            logger.MarkupLine($"[red]Passwords do not match[/]. Please try again. You have [yellow]{i - 1} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }
}
