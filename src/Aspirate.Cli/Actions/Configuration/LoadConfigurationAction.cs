namespace Aspirate.Cli.Actions.Configuration;

public class LoadConfigurationAction(
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "LoadConfigurationAction";

    public override Task<bool> ExecuteAsync()
    {
        var aspirateSettings = configurationService.LoadConfigurationFile(CurrentState.InputParameters.AspireManifestPath);

        if (aspirateSettings is not null)
        {
            CurrentState.InputParameters.LoadedAspirateSettings = aspirateSettings;
            Logger.MarkupLine($"\r\n[bold] Successfully loaded existing aspirate bootstrap settings from [blue]'{CurrentState.InputParameters.AspireManifestPath}'[/].[/]");
        }

        return Task.FromResult(true);
    }
}
