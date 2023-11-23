namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand(
    IAnsiConsole console,
    IAspirateConfigurationService aspirateConfigurationService,
    IServiceProvider serviceProvider) : AsyncCommand<InitInput>
{
    public const string InitCommandName = "init";
    public const string InitDescription = "Initializes aspirate settings within your AppHost directory.";

    public override Task<int> ExecuteAsync(CommandContext context, InitInput settings)
    {
        aspirateConfigurationService.HandleExistingConfiguration(settings.PathToAspireProjectFlag);

        var aspirateSettings = PerformConfigurationBootstrapping();

        aspirateConfigurationService.SaveConfigurationFile(aspirateSettings, settings.PathToAspireProjectFlag);

        console.LogCommandCompleted();

        return Task.FromResult(0);
    }

    private AspirateSettings PerformConfigurationBootstrapping()
    {
        var aspirateConfiguration = new AspirateSettings();

        HandleContainerRegistry(aspirateConfiguration);

        HandleContainerTag(aspirateConfiguration);

        HandleTemplateDirectory(aspirateConfiguration);

        return aspirateConfiguration;
    }

    private void HandleContainerRegistry(AspirateSettings aspirateConfiguration)
    {
        console.MarkupLine("\r\nAspirate supports setting a fall-back value for projects that have not yet set a [blue]'ContainerRegistry'[/] in their csproj file.");
        var shouldSetContainerRegistry = console.Confirm("Would you like to set a fall-back value for the container registry?", false);

        if (!shouldSetContainerRegistry)
        {
            return;
        }

        var containerRegistry = console.Prompt(new TextPrompt<string>("\tPlease enter the container registry to use as a fall-back value:").PromptStyle("blue"));
        aspirateConfiguration.ContainerSettings.Registry = containerRegistry;
    }

    private void HandleContainerTag(AspirateSettings aspirateConfiguration)
    {
        console.MarkupLine("\r\nAspirate supports setting a fall-back value for projects that have not yet set a [blue]'ContainerTag'[/] in their csproj file.");
        var shouldSetContainerTag = console.Confirm("Would you like to set a fall-back value for the container tag?", false);

        if (!shouldSetContainerTag)
        {
            return;
        }

        var containerTag = console.Prompt(new TextPrompt<string>("\tPlease enter the container tag to use as a fall-back value:").PromptStyle("blue"));
        aspirateConfiguration.ContainerSettings.Tag = containerTag;
        console.WriteLine();
    }

    private void HandleTemplateDirectory(AspirateSettings aspirateConfiguration)
    {
        console.MarkupLine("\r\nAspirate supports setting a custom directory for [blue]'Templates'[/] that are used when generating kustomize manifests.");
        var shouldSetCustomTemplateDirectory = console.Confirm("Would you like to use a custom directory (selecting [green]'n'[/] will default to built in templates ?", false);

        if (!shouldSetCustomTemplateDirectory)
        {
            return;
        }

        var templatePath = console.Prompt(new TextPrompt<string>("\tPlease enter the path to use as the template directory:").PromptStyle("blue"));
        aspirateConfiguration.TemplatePath = templatePath;
        console.WriteLine();
    }
}
