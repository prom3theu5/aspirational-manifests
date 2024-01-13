namespace Aspirate.Commands.Actions.Configuration;

public class LoadConfigurationAction(
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        var aspirateSettings = configurationService.LoadConfigurationFile(CurrentState.ProjectPath);

        if (aspirateSettings is null)
        {
            return Task.FromResult(true);
        }

        CurrentState.TemplatePath = aspirateSettings.TemplatePath ?? null;
        CurrentState.ContainerRegistry = aspirateSettings.ContainerSettings?.Registry ?? null;
        CurrentState.ContainerBuilder = aspirateSettings.ContainerSettings?.Builder ?? null;
        CurrentState.ContainerRepositoryPrefix = aspirateSettings.ContainerSettings?.RepositoryPrefix ?? null;
        CurrentState.ContainerImageTag = aspirateSettings.ContainerSettings?.Tag ?? null;
        Logger.MarkupLine($"\r\n[bold]Successfully loaded existing aspirate bootstrap settings from [blue]'{CurrentState.ProjectPath}'[/].[/]");
        Logger.WriteLine();

        return Task.FromResult(true);
    }
}
