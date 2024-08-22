namespace Aspirate.Commands.Actions.Secrets;

public sealed class PopulateInputsWithEnvVariablesAction(IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Generating enviroment variable placeholders for inputs[/]");

        var parameterResources = CurrentState.LoadedAspireManifestResources.Where(x => x.Value is ParameterResource).ToArray();

        if (parameterResources.Length == 0)
        {
            return Task.FromResult(true);
        }

        foreach (var parameter in parameterResources)
        {
            if (parameter.Value is ParameterResource resource)
            {
                resource.Value = $"${{{resource.Name.Replace('-', '_').ToUpperInvariant()}}}";
            }
        }

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] Input values have all been assigned enviroment variables.");

        return Task.FromResult(true);
    }
}
