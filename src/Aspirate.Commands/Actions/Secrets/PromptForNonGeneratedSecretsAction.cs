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

            AssignManualValues(ref manualInputs);
            AssignGeneratedValues(ref generatedInputs);
        }

        return Task.FromResult(true);
    }

    private void AssignManualValues(ref IEnumerable<KeyValuePair<string, Input>>? manualInputs)
    {
        if (manualInputs is null)
        {
            return;
        }

        foreach (var input in manualInputs)
        {
            input.Value.Value = console.Prompt(
                new TextPrompt<string>("\r\nEnter a value for {component.Key} : {input.Key}")
                    .PromptStyle("red")
                    .Secret());
        }
    }

    private void AssignGeneratedValues(ref IEnumerable<KeyValuePair<string,Input>>? generatedInputs)
    {
        if (generatedInputs is null)
        {
            return;
        }

        foreach (var input in generatedInputs)
        {
            var minimumLength = input.Value.Default?.Generate?.MinLength ?? 16;
            input.Value.Value = passwordGenerator.Generate(minimumLength);
        }
    }
}
