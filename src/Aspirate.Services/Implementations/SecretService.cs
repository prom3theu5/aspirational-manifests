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

        if (secretProvider.SecretStateExists(options.State))
        {
            logger.MarkupLine("Aspirate Secrets [blue]already exist[/] for manifest.");

            secretProvider.LoadState(options.State);

            if (!CheckPassword(options))
            {
                logger.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                ActionCausesExitException.ExitNow();
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
                        logger.MarkupLine($"Using [green]existing[/] secrets.");
                        return;
                    case Augment:
                        logger.MarkupLine(
                            $"Using [green]existing[/] secrets and augmenting with new values.");
                        break;
                    case Overwrite:
                        logger.MarkupLine($"[yellow]Overwriting[/] secrets");
                        secretProvider.RemoveState(options.State);
                        break;
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
                strategy.ProtectSecrets(component, options.NonInteractive);
            }
        }

        secretProvider.SetState(options.State);

        logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State has been saved.");
    }

    public void LoadSecrets(SecretManagementOptions options)
    {
        if (options.DisableSecrets)
        {
            logger.MarkupLine("[green]Secrets are disabled[/].");
            return;
        }

        if (!secretProvider.SecretStateExists(options.State))
        {
            logger.MarkupLine("[green]No existing secrets state found[/].");
            return;
        }

        if (options.NonInteractive)
        {
            if (string.IsNullOrEmpty(options.SecretPassword))
            {
                logger.ValidationFailed("Secrets are protected by a password, but no password has been provided.");
            }
        }

        secretProvider.LoadState(options.State);

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
