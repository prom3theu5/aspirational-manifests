namespace Aspirate.Commands.Actions.Secrets;

public class SaveSecretsAction(
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider,
    IEnumerable<ISecretProtectionStrategy> protectionStrategies) : BaseAction(serviceProvider)
{
    private const string UseExisting = "Use Existing";
    private const string Augment = "Augment by adding / replacing values";
    private const string Overwrite = "Overwrite / Create new Password";

    private IReadOnlyCollection<ISecretProtectionStrategy> ProtectionStrategies { get; } = protectionStrategies.ToList();

    public override Task<bool> ExecuteAsync()
    {
        if (CurrentState.DisableSecrets)
        {
            return Task.FromResult(true);
        }

        if (!ProtectionStrategies.CheckForProtectableSecrets(CurrentState.AllSelectedSupportedComponents))
        {
            console.MarkupLine("No secrets to protect in any [blue]selected components[/]");

            return Task.FromResult(true);
        }

        if (secretProvider.SecretStateExists(CurrentState.OutputPath))
        {
            console.MarkupLine("Aspirate Secrets [blue]already exist[/] for manifest.");

            secretProvider.LoadState(CurrentState.OutputPath);

            if (secretProvider is PasswordSecretProvider passwordSecretProvider)
            {
                var (correctPassword, password) = CheckPassword(passwordSecretProvider);
                if (!correctPassword)
                {
                    console.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                    ActionCausesExitException.ExitNow();
                }

                passwordSecretProvider.SetPassword(password!);
            }

            if (!CurrentState.NonInteractive)
            {
                var secretsAction = console.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select the action for the existing secrets...")
                        .HighlightStyle("blue")
                        .PageSize(3)
                        .AddChoices(UseExisting, Augment, Overwrite));

                switch (secretsAction)
                {
                    case UseExisting:
                        console.MarkupLine($"Using [green]existing[/] secrets for provider [blue]{secretProvider.Type}[/]");
                        return Task.FromResult(true);
                    case Augment:
                        console.MarkupLine(
                            $"Using [green]existing[/] secrets for provider [blue]{secretProvider.Type}[/] and augmenting with new values.");
                        break;
                    case Overwrite:
                        console.MarkupLine($"[yellow]Overwriting[/] secrets for provider [blue]{secretProvider.Type}[/]");
                        secretProvider.RemoveState(CurrentState.OutputPath);
                        break;
                }
            }
        }

        if (!secretProvider.SecretStateExists(CurrentState.OutputPath))
        {
            HandleInitialisation();
        }

        foreach (var component in CurrentState.AllSelectedSupportedComponents.Where(component => !secretProvider.ResourceExists(component.Key)))
        {
            secretProvider.AddResource(component.Key);
        }

        foreach (var component in CurrentState.AllSelectedSupportedComponents)
        {
            if (component.Value.Env is null)
            {
                continue;
            }

            foreach (var strategy in ProtectionStrategies)
            {
                strategy.ProtectSecrets(component, CurrentState.NonInteractive);
            }
        }

        secretProvider.SaveState(CurrentState.OutputPath);

        console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State has been saved to [blue]{CurrentState.OutputPath}/{AspirateSecretLiterals.SecretsStateFile}[/]");

        return Task.FromResult(true);
    }



    private void HandleInitialisation()
    {
        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            var result = CreatePassword(passwordSecretProvider);
            if (!result)
            {
                console.MarkupLine("[red]Aborting due to inability to create password.[/]");
                ActionCausesExitException.ExitNow();
            }
        }
    }

    private bool CreatePassword(PasswordSecretProvider passwordSecretProvider)
    {
        console.MarkupLine("Secrets are to be protected by a [green]password[/]");

        if (!string.IsNullOrEmpty(CurrentState.SecretPassword))
        {
            passwordSecretProvider.SetPassword(CurrentState.SecretPassword);
            return true;
        }

        for (int i = 3; i > 0; i--)
        {
            console.WriteLine();
            var firstEntry = console.Prompt(
                new TextPrompt<string>("Please enter new Password: ")
                    .PromptStyle("red")
                    .Secret());

            var secondEntry = console.Prompt(
                new TextPrompt<string>("Please enter it again to confirm: ")
                    .PromptStyle("red")
                    .Secret());

            if (firstEntry.Equals(secondEntry, StringComparison.Ordinal))
            {
                passwordSecretProvider.SetPassword(firstEntry);
                return true;
            }

            console.MarkupLine($"[red]Passwords do not match[/]. Please try again. You have [yellow]{i - 1} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }

    private (bool Success, string? Password) CheckPassword(PasswordSecretProvider passwordSecretProvider)
    {
        console.MarkupLine("Existing Secrets are protected by a [green]password[/].");

        if (CliSecretPasswordSupplied(passwordSecretProvider, out var validPassword))
        {
            return (validPassword, CurrentState.SecretPassword);
        }

        for (int i = 3; i > 0; i--)
        {
            console.WriteLine();
            var password = console.Prompt(
                new TextPrompt<string>("Please enter it now to confirm secret actions: ").PromptStyle("red")
                    .Secret());

            if (passwordSecretProvider.CheckPassword(password))
            {
                return (true, password);
            }

            console.MarkupLine($"[red]Incorrect password[/]. Please try again. You have [yellow]{i - 1} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return (false, null);
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
}
