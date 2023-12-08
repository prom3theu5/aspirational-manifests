namespace Aspirate.Commands.Actions.Secrets;

public class PromptForNonGeneratedSecretsAction(
    IAnsiConsole console,
    IPasswordGenerator passwordGenerator,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (CurrentState.NonInteractive)
        {
            // Values cannot be prompted for in non-interactive mode.
            return Task.FromResult(true);
        }

        var componentsWithInputs = CurrentState.AllSelectedSupportedComponents.Where(x => x.Value is IResourceWithInput).ToArray();

        if (componentsWithInputs.Length == 0)
        {
            return Task.FromResult(true);
        }

        console.MarkupLine("You will now be prompted to enter values for any [green]secrets[/] that are [blue]not generated[/] automatically.");
        console.MarkupLine("All [blue]automatically generated[/] [green]secrets[/] will be created accordingly.");

        foreach (var component in componentsWithInputs)
        {
            var componentWithInput = component.Value as IResourceWithInput;

            var generatedInputs = componentWithInput.Inputs?.Where(x => x.Value.Default?.Generate is not null);
            var manualInputs = componentWithInput.Inputs?.Where(x => x.Value.Default?.Generate is null);

            AssignManualValues(ref manualInputs, componentWithInput);
            AssignGeneratedValues(ref generatedInputs, componentWithInput);
        }

        return Task.FromResult(true);
    }

    private void AssignManualValues(ref IEnumerable<KeyValuePair<string, Input>>? manualInputs,
        IResourceWithInput componentWithInput)
    {
        if (manualInputs is null)
        {
            return;
        }

        foreach (var input in manualInputs)
        {
            HandleSetInput(input, componentWithInput);
        }
    }

    private void HandleSetInput(KeyValuePair<string, Input> input, IResourceWithInput componentWithInput)
    {
        console.WriteLine();

        var firstPrompt = new TextPrompt<string>($"\r\nEnter a value for resource [blue]{componentWithInput.Name}'s[/] Input Value [blue]'{input.Key}'[/]: ");
        var secondPrompt = new TextPrompt<string>("\r\nPlease repeat the value: ");

        if (input.Value.Secret)
        {
            firstPrompt.PromptStyle("red").Secret();
            secondPrompt.PromptStyle("red").Secret();
        }
        else
        {
            firstPrompt.PromptStyle("yellow");
            secondPrompt.PromptStyle("yellow");
        }

        var firstInput = console.Prompt(firstPrompt);
        var secondInput = console.Prompt(secondPrompt);

        if (firstInput.Equals(secondInput, StringComparison.Ordinal))
        {
            input.Value.Value = firstInput;
            return;
        }

        console.MarkupLine("[red]The values do not match. Please try again.[/]");
        HandleSetInput(input, componentWithInput);
    }

    private void AssignGeneratedValues(ref IEnumerable<KeyValuePair<string, Input>>? generatedInputs,
        IResourceWithInput componentWithInput)
    {
        if (generatedInputs is null)
        {
            return;
        }

        foreach (var input in generatedInputs)
        {
            console.WriteLine();
            var minimumLength = input.Value.Default?.Generate?.MinLength ?? 16;
            input.Value.Value = passwordGenerator.Generate(minimumLength);

            console.MarkupLine($"Successfully [green]generated[/] a value for [blue]{componentWithInput.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
        }
    }
}
