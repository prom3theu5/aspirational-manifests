namespace Aspirate.Secrets.Protectors;

public abstract class BaseProtector(ISecretProvider secretProvider, IAnsiConsole console) : ISecretProtectionStrategy
{
    private const string UseExisting = "Use Existing";
    private const string Replace = "Replace";

    public abstract bool HasSecrets(KeyValuePair<string, Resource> component);

    public abstract void ProtectSecrets(KeyValuePair<string, Resource> component);

    protected void UpsertSecret(KeyValuePair<string, Resource> component, KeyValuePair<string, string> input)
    {
        var secretExists = secretProvider.SecretExists(component.Key, input.Key);

        bool replaced = false;

        if (secretExists)
        {
            console.MarkupLine($"[yellow]Secret for [blue]{component.Key}[/] > [blue]{input.Key}[/] already exists[/]");

            var secretAction = console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the action for the secret...")
                    .PageSize(3)
                    .AddChoices(UseExisting, Replace));

            if (secretAction.Equals(UseExisting, StringComparison.OrdinalIgnoreCase))
            {
                console.MarkupLine($"Using [green]existing[/] secret for {component.Key} {input.Key}");
                return;
            }

            secretProvider.RemoveSecret(component.Key, input.Key);
            replaced = true;
        }

        console.MarkupLine($"Secret for [blue]{component.Key}[/] > [blue]{input.Key}[/] has been [green]{(replaced ? "replaced" : "added")}[/]");
        secretProvider.AddSecret(component.Key, input.Key, input.Value);
    }
}
