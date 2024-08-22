namespace Aspirate.Commands.Actions.Secrets;
public sealed class PopulateInputsAction(
    IPasswordGenerator passwordGenerator,
    IServiceProvider serviceProvider,
    ISecretProvider secretProvider) : BaseAction(serviceProvider)
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

        ApplyManualValues(parameterResources);

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Input values have all been assigned.");

        return Task.FromResult(true);
    }

    private void ApplyManualValues(KeyValuePair<string, Resource>[] parameterResources)
    {
        foreach (var component in parameterResources)
        {
            var componentWithInput = component.Value as ParameterResource;

            var manualInputs = componentWithInput.Inputs?.Where(x => x.Value.Default is null);

            AssignManualValues(ref manualInputs, componentWithInput);
        }
    }

    private void ApplyGeneratedValues(KeyValuePair<string, Resource>[] parameterResources)
    {
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
        if (AssignExistingSecret(input, parameterResource))
        {
            return;
        }

        if (CurrentState.NonInteractive)
        {
            Logger.ValidationFailed("Cannot obtain non-generated values for inputs in non-interactive mode. Inputs are required according to the manifest.");
            ActionCausesExitException.ExitNow();
        }

        var firstPrompt = new TextPrompt<string>($"Enter a value for resource [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]: ").PromptStyle("yellow");
        var secondPrompt = new TextPrompt<string>("Please repeat the value: ").PromptStyle("yellow");

        var firstInput = Logger.Prompt(firstPrompt);
        var secondInput = Logger.Prompt(secondPrompt);

        if (firstInput.Equals(secondInput, StringComparison.Ordinal))
        {
            parameterResource.Value = firstInput;
            AddParameterInputToSecretStore(input, parameterResource, firstInput);
            Logger.MarkupLine($"Successfully [green]assigned[/] a value for [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
            return;
        }

        Logger.MarkupLine("[red]The values do not match. Please try again.[/]");
        HandleSetInput(input, parameterResource);
    }

    private bool AssignExistingSecret(KeyValuePair<string, ParameterInput> input, ParameterResource parameterResource)
    {
        if (CurrentState.ReplaceSecrets == true || CurrentState.DisableSecrets == true || !secretProvider.SecretStateExists(CurrentState) || !secretProvider.ResourceExists(parameterResource.Name) ||
            !secretProvider.SecretExists(parameterResource.Name, input.Key))
        {
            return false;
        }

        parameterResource.Value = secretProvider.GetSecret(parameterResource.Name, input.Key);
        Logger.MarkupLine(
            $"[green]Secret[/] for [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/] loaded from secret state.");

        return true;
    }

    private void AddParameterInputToSecretStore(KeyValuePair<string, ParameterInput> input, ParameterResource parameterResource, string valueToStore)
    {
        if (CurrentState.DisableSecrets == true)
        {
            return;
        }

        if (!secretProvider.ResourceExists(parameterResource.Name))
        {
            secretProvider.AddResource(parameterResource.Name);
        }

        secretProvider.AddSecret(parameterResource.Name, input.Key, valueToStore);
    }

    private void AssignGeneratedValues(ref IEnumerable<KeyValuePair<string, ParameterInput>>? generatedInputs, ParameterResource parameterResource)
    {
        if (generatedInputs is null)
        {
            return;
        }

        foreach (var input in generatedInputs)
        {
            if (AssignExistingSecret(input, parameterResource))
            {
                continue;
            }
            var minimumLength = input.Value.Default?.Generate?.MinLength ?? 22;
            parameterResource.Value = passwordGenerator.Generate(minimumLength);
            AddParameterInputToSecretStore(input, parameterResource, parameterResource.Value);

            Logger.MarkupLine($"Successfully [green]generated[/] a value for [blue]{parameterResource.Name}'s[/] Input Value [blue]'{input.Key}'[/]");
        }
    }
}
