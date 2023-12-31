namespace Aspirate.Commands.Actions.Secrets;

public class PopulateInputsAction(
    IPasswordGenerator passwordGenerator,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        var componentsWithInputs = CurrentState.AllSelectedSupportedComponents.Where(x => x.Value is IResourceWithInput).ToArray();

        if (componentsWithInputs.Length == 0)
        {
            return Task.FromResult(true);
        }

        ApplyGeneratedValues(componentsWithInputs);

        if (CurrentState.NonInteractive)
        {
            return Task.FromResult(true);
        }

        ApplyManualValues(componentsWithInputs);

        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done: [/] Input values have all been assigned.");

        return Task.FromResult(true);
    }

    private void ApplyManualValues(KeyValuePair<string, Resource>[] componentsWithInputs)
    {
        Logger.MarkupLine("You will now be prompted to enter values for any [green]secrets[/] that are [blue]not generated[/] automatically.");

        foreach (var component in componentsWithInputs)
        {
            var componentWithInput = component.Value as IResourceWithInput;

            var manualInputs = componentWithInput.Inputs?.Where(x => x.Value.Default?.Generate is null);

            AssignManualValues(ref manualInputs, componentWithInput);
        }
    }

    private void ApplyGeneratedValues(KeyValuePair<string, Resource>[] componentsWithInputs)
    {
        Logger.WriteLine();
        Logger.MarkupLine("Applying values for all [blue]automatically generated[/] [green]secrets[/].");

        foreach (var component in componentsWithInputs)
        {
            var componentWithInput = component.Value as IResourceWithInput;

            var generatedInputs = componentWithInput.Inputs?.Where(x => x.Value.Default?.Generate is not null);

            AssignGeneratedValues(ref generatedInputs, componentWithInput);
        }
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
        Logger.WriteLine();

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

        var firstInput = Logger.Prompt(firstPrompt);
        var secondInput = Logger.Prompt(secondPrompt);

        if (firstInput.Equals(secondInput, StringComparison.Ordinal))
        {
            input.Value.Value = firstInput;
            return;
        }

        Logger.MarkupLine("[red]The values do not match. Please try again.[/]");
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
            Logger.WriteLine();
            var minimumLength = input.Value.Default?.Generate?.MinLength ?? 16;
            input.Value.Value = passwordGenerator.Generate(minimumLength);

            Logger.MarkupLine($"Successfully [green]generated[/] a value for [blue]{componentWithInput.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
        }
    }

    public override void ValidateNonInteractiveState()
    {
        var componentsWithInputs = CurrentState.AllSelectedSupportedComponents.Where(x => x.Value is IResourceWithInput).ToArray();

        var manualInputs = componentsWithInputs
            .Select(x => (IResourceWithInput) x.Value)
            .Where(x=>x.Inputs is not null)
            .SelectMany(x => x.Inputs)
            .Where(x => x.Value.Default?.Generate is null);

        if (manualInputs.Any())
        {
            NonInteractiveValidationFailed("Cannot obtain non-generated values for inputs in non-interactive mode. Inputs are required according to the manifest.");
        }
    }
}
