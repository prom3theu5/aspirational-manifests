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

        if (string.IsNullOrEmpty(CurrentState.TemplatePath))
        {
            CurrentState.TemplatePath = aspirateSettings.TemplatePath ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerRegistry))
        {
            CurrentState.ContainerRegistry = aspirateSettings.ContainerSettings?.Registry ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerBuilder))
        {
            CurrentState.ContainerBuilder = aspirateSettings.ContainerSettings?.Builder ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerRepositoryPrefix))
        {
            CurrentState.ContainerRepositoryPrefix = aspirateSettings.ContainerSettings?.RepositoryPrefix ?? null;
        }

        if (string.IsNullOrEmpty(CurrentState.ContainerImageTag))
        {
            CurrentState.ContainerImageTag = aspirateSettings.ContainerSettings?.Tag ?? null;
        }

        Logger.MarkupLine($"\r\n[bold]Successfully loaded existing aspirate bootstrap settings from [blue]'{CurrentState.ProjectPath}'[/].[/]");
        Logger.WriteLine();

        return Task.FromResult(true);
    }
}
