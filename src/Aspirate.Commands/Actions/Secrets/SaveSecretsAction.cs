namespace Aspirate.Commands.Actions.Secrets;

public class SaveSecretsAction(
    IAnsiConsole console,
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (secretProvider.SecretStateExists(CurrentState.OutputPath))
        {
            console.MarkupLine($"[yellow]Secrets for provider {secretProvider.Type} already exist[/]");

            secretProvider.LoadState(CurrentState.OutputPath);

            if (secretProvider is PasswordSecretProvider passwordSecretProvider)
            {
                var correctPassword = CheckPassword(passwordSecretProvider);
                if (!correctPassword)
                {
                    console.MarkupLine("[red]Aborting due to inability to unlock secrets.[/]");
                    throw new ActionCausesExitException(1);
                }
            }

            if (AskIfShouldUseExisting(plural: true))
            {
                console.MarkupLine($"Using [green]existing[/] secrets for provider [blue]{secretProvider.Type}[/]");
                return Task.FromResult(true);
            }

            if (!AskIfShouldOverwrite(plural: true, defaultValue: false))
            {
                console.MarkupLine("[red]Aborting due to inability to modify secrets.[/]");
                throw new ActionCausesExitException(1);
            }
        }

        if (!secretProvider.SecretStateExists(CurrentState.OutputPath))
        {
            HandleInitialisation();
        }

        console.MarkupLine($"Saving secrets for provider [blue]{secretProvider.Type}[/]");

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

            ProtectConnectionStrings(component);
        }

        secretProvider.SaveState(CurrentState.OutputPath);

        console.MarkupLine($"\r\n\t[green]({EmojiLiterals.CheckMark}) Done: [/] Secret State has been saved to [blue]{CurrentState.OutputPath}/{AspirateSecretLiterals.SecretsStateFile}[/]");

        return Task.FromResult(true);
    }

    private void UpsertSecret(KeyValuePair<string, Resource> component, KeyValuePair<string, string> input)
    {
        if (secretProvider.SecretExists(component.Key, input.Key))
        {
            console.MarkupLine($"[yellow]Secret for {component.Key} {input.Key} already exists[/]");

            if (AskIfShouldUseExisting())
            {
                return;
            }

            if (!AskIfShouldOverwrite())
            {
                console.MarkupLine("[red]Aborting due to inability to modify secret.[/]");
                throw new ActionCausesExitException(1);
            }

            secretProvider.RemoveSecret(component.Key, input.Key);
        }

        secretProvider.AddSecret(component.Key, input.Key, input.Value);
    }

    private void HandleInitialisation()
    {
        if (secretProvider is PasswordSecretProvider passwordSecretProvider)
        {
            var result = CreatePassword(passwordSecretProvider);
            if (!result)
            {
                console.MarkupLine("[red]Aborting due to inability to create password.[/]");
                throw new ActionCausesExitException(1);
            }
        }
    }

    private bool CreatePassword(PasswordSecretProvider passwordSecretProvider)
    {
        for (int i = 0; i < 3; i++)
        {
            var firstEntry = console.Prompt(
                new TextPrompt<string>("Secrets are to be protected by a [green]password[/]. Please enter it now: ")
                    .PromptStyle("red")
                    .Secret());

            var secondEntry = console.Prompt(
                new TextPrompt<string>("Please enter it again to confirm: ")
                    .PromptStyle("red")
                    .Secret());

            if (firstEntry == secondEntry)
            {
                passwordSecretProvider.SetPassword(firstEntry);
                return true;
            }

            console.MarkupLine($"[red]Passwords do not match[/]. Please try again. You have [yellow]{3 - i} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }

    private bool CheckPassword(PasswordSecretProvider passwordSecretProvider)
    {
        for (int i = 0; i < 3; i++)
        {
            var password = console.Prompt(
                new TextPrompt<string>("Secrets are protected by a [green]password[/]. Please enter it now: ").PromptStyle("red")
                    .Secret());

            if (passwordSecretProvider.CheckPassword(password))
            {
                return true;
            }

            console.MarkupLine($"[red]Incorrect password[/]. Please try again. You have [yellow]{3 - i} attempt{(i > 1 ? "s" : "")}[/] remaining.");
        }

        return false;
    }

    private void ProtectConnectionStrings(KeyValuePair<string, Resource> component)
    {
        var connectionStrings = component.Value.Env?.Where(x => x.Key.StartsWith("ConnectionString", StringComparison.OrdinalIgnoreCase)).ToList();

        if (connectionStrings.Count == 0)
        {
            return;
        }

        foreach (var input in connectionStrings)
        {
            UpsertSecret(component, input);
        }
    }

    private bool AskIfShouldOverwrite(bool plural = false, bool defaultValue = true) =>
        console.Confirm($"Do you want to [red]overwrite[/] {(plural ? "them" : "it")}?", defaultValue: defaultValue);

    private bool AskIfShouldUseExisting(bool plural = false) =>
        console.Confirm($"Do you want to use the [blue]existing[/] secret{(plural ? "s" : "")}?");
}
