namespace Aspirate.Commands.Actions.Configuration;

public class LoadConfigurationAction(
    IAspirateConfigurationService configurationService,
    ISecretProvider secretProvider,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        //todo: Replace aspiratesettings with an instance of the state, also pass down the loaded json inside the LoadConfigurationFile method into the registered secret provider to populated the secrets.

        var aspirateSettings = configurationService.LoadConfigurationFile(CurrentState.ProjectPath);

        if (aspirateSettings is not null)
        {
            CurrentState.TemplatePath = aspirateSettings.TemplatePath ?? null;
            CurrentState.ContainerRegistry = aspirateSettings.ContainerSettings?.Registry ?? null;
            CurrentState.ContainerImageTag = aspirateSettings.ContainerSettings?.Tag ?? null;
            Logger.MarkupLine($"\r\n[bold]Successfully loaded existing aspirate bootstrap settings from [blue]'{CurrentState.ProjectPath}'[/].[/]");
            Logger.WriteLine();
        }

        return Task.FromResult(true);
    }
}
