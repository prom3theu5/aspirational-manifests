namespace Aspirate.Commands.Actions.Secrets;

public class PopulateInputsAction(
    IPasswordGenerator passwordGenerator,
    IServiceProvider serviceProvider) : BaseActionWithNonInteractiveValidation(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling Inputs[/]");

        var parameterResources = CurrentState.LoadedAspireManifestResources.Where(x => x.Value is ParameterResource).ToArray();

        if (parameterResources.Length == 0)
        {
            return Task.FromResult(true);
        }

        ApplyGeneratedValues(parameterResources);

        if (CurrentState.NonInteractive)
        {
            return Task.FromResult(true);
        }

        ApplyManualValues(parameterResources);

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Input values have all been assigned.");

        return Task.FromResult(true);
    }

    private void ApplyManualValues(KeyValuePair<string, Resource>[] parameterResources)
    {
        Logger.MarkupLine("You will now be prompted to enter values for any [green]secrets[/] that are [blue]not generated[/] automatically.");

        foreach (var component in parameterResources)
        {
            var componentWithInput = component.Value as ParameterResource;

            var manualInputs = componentWithInput.Inputs?.Where(x => x.Value.Default is null);

            AssignManualValues(ref manualInputs, componentWithInput);
        }
    }

    private void ApplyGeneratedValues(KeyValuePair<string, Resource>[] parameterResources)
    {
        Logger.MarkupLine("Applying values for all [blue]automatically generated[/] [green]secrets[/].");

        foreach (var component in parameterResources)
        {
            var componentWithInput = component.Value as ParameterResource;

            var generatedInputs = componentWithInput.Inputs?.Where(x => x.Value.Default is not null);

            AssignGeneratedValues(ref generatedInputs, componentWithInput);
        }
    }

    private void AssignManualValues(ref IEnumerable<KeyValuePair<string, ParameterInput>>? manualInputs, ParameterResource parameterResource)
    {
        if (manualInputs is null)
        {
            return;
        }

        foreach (var input in manualInputs)
        {
            HandleSetInput(input, parameterResource);
        }
    }

    private void HandleSetInput(KeyValuePair<string, ParameterInput> input, ParameterResource parameterResource)
    {
        Logger.WriteLine();

        var firstPrompt = new TextPrompt<string>($"Enter a value for resource [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]: ");
        var secondPrompt = new TextPrompt<string>("Please repeat the value: ");

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
            parameterResource.Value = firstInput;
            return;
        }

        Logger.MarkupLine("[red]The values do not match. Please try again.[/]");
        HandleSetInput(input, parameterResource);
    }

    private void AssignGeneratedValues(ref IEnumerable<KeyValuePair<string, ParameterInput>>? generatedInputs, ParameterResource parameterResource)
    {
        if (generatedInputs is null)
        {
            return;
        }

        foreach (var input in generatedInputs)
        {
            Logger.WriteLine();
            var minimumLength = input.Value.Default?.Generate?.MinLength ?? 22;
            parameterResource.Value = passwordGenerator.Generate(minimumLength);

            Logger.MarkupLine($"Successfully [green]generated[/] a value for [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
        }
    }

    public override void ValidateNonInteractiveState()
    {
        var componentsWithInputs = CurrentState.AllSelectedSupportedComponents.Where(x => x.Value is ParameterResource).ToArray();

        var manualInputs = componentsWithInputs
            .Select(x => (ParameterResource) x.Value)
            .Where(x => x.Inputs is not null)
            .SelectMany(x => x.Inputs)
            .Where(x => x.Value.Default is null);

        if (manualInputs.Any())
        {
            Logger.ValidationFailed("Cannot obtain non-generated values for inputs in non-interactive mode. Inputs are required according to the manifest.");
        }
    }
}
