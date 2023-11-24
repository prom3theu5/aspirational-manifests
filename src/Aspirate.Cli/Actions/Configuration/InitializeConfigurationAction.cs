namespace Aspirate.Cli.Actions.Configuration;

public class InitializeConfigurationAction(
    IFileSystem fileSystem,
    IAspirateConfigurationService configurationService,
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public const string ActionKey = "InitializeConfigurationAction";

    public override Task<bool> ExecuteAsync()
    {
        configurationService.HandleExistingConfiguration(CurrentState.InputParameters.AspireManifestPath);

        var aspirateSettings = PerformConfigurationBootstrapping();

        configurationService.SaveConfigurationFile(aspirateSettings, CurrentState.InputParameters.AspireManifestPath);

        return Task.FromResult(true);
    }

    private AspirateSettings PerformConfigurationBootstrapping()
    {
        var aspirateConfiguration = new AspirateSettings();

        HandleContainerRegistry(aspirateConfiguration);

        HandleContainerTag(aspirateConfiguration);

        HandleTemplateDirectory(aspirateConfiguration);

        AddTemplatesToTemplateDirectoryIfRequired(aspirateConfiguration);

        return aspirateConfiguration;
    }

    private void HandleContainerRegistry(AspirateSettings aspirateConfiguration)
    {
        Logger.MarkupLine("\r\nAspirate supports setting a fall-back value for projects that have not yet set a [blue]'ContainerRegistry'[/] in their csproj file.");
        var shouldSetContainerRegistry = Logger.Confirm("Would you like to set a fall-back value for the container registry?", false);

        if (!shouldSetContainerRegistry)
        {
            return;
        }

        var containerRegistry = Logger.Prompt(new TextPrompt<string>("\tPlease enter the container registry to use as a fall-back value:").PromptStyle("blue"));
        aspirateConfiguration.ContainerSettings.Registry = containerRegistry;
        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'Container fallback registry'[/] to [blue]'{aspirateConfiguration.ContainerSettings.Registry}'[/].");
        Logger.WriteLine();
    }

    private void HandleContainerTag(AspirateSettings aspirateConfiguration)
    {
        Logger.MarkupLine("\r\nAspirate supports setting a fall-back value for projects that have not yet set a [blue]'ContainerTag'[/] in their csproj file.");
        var shouldSetContainerTag = Logger.Confirm("Would you like to set a fall-back value for the container tag?", false);

        if (!shouldSetContainerTag)
        {
            return;
        }

        var containerTag = Logger.Prompt(new TextPrompt<string>("\tPlease enter the container tag to use as a fall-back value:").PromptStyle("blue"));
        aspirateConfiguration.ContainerSettings.Tag = containerTag;
        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'Container fallback tag'[/] to [blue]'{aspirateConfiguration.ContainerSettings.Tag}'[/].");
        Logger.WriteLine();
    }

    private void HandleTemplateDirectory(AspirateSettings aspirateConfiguration)
    {
        Logger.MarkupLine("\r\nAspirate supports setting a custom directory for [blue]'Templates'[/] that are used when generating kustomize manifests.");
        var shouldSetCustomTemplateDirectory = Logger.Confirm("Would you like to use a custom directory (selecting [green]'n'[/] will default to built in templates ?", false);

        if (!shouldSetCustomTemplateDirectory)
        {
            return;
        }

        var templatePath = Logger.Prompt(new TextPrompt<string>("\tPlease enter the path to use as the template directory:").PromptStyle("blue"));
        aspirateConfiguration.TemplatePath = fileSystem.GetFullPath(templatePath);
        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'TemplatePath'[/] to [blue]'{aspirateConfiguration.TemplatePath}'[/].");
        Logger.WriteLine();
    }

    private void AddTemplatesToTemplateDirectoryIfRequired(AspirateSettings aspirateConfiguration)
    {
        if (string.IsNullOrEmpty(aspirateConfiguration.TemplatePath))
        {
            return;
        }

        var templateDirectoryExists = fileSystem.Directory.Exists(aspirateConfiguration.TemplatePath);

        if (!templateDirectoryExists)
        {
            templateDirectoryExists = HandleCreateTemplateFolder(aspirateConfiguration);
        }

        // said no to creating.
        if (!templateDirectoryExists)
        {
            return;
        }

        HandleCreateTemplateFiles(aspirateConfiguration);
    }

    private bool HandleCreateTemplateFolder(AspirateSettings aspirateConfiguration)
    {
        Logger.MarkupLine($"\r\nSelected Template directory [blue]'{aspirateConfiguration.TemplatePath}'[/] does not exist.");
        var shouldCreate = Logger.Confirm("Would you like to create it?");

        if (!shouldCreate)
        {
            return false;
        }

        fileSystem.Directory.CreateDirectory(aspirateConfiguration.TemplatePath);
        Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Directory [blue]'{aspirateConfiguration.TemplatePath}'[/] has been created.");
        return true;
    }

    private void HandleCreateTemplateFiles(AspirateSettings aspirateConfiguration)
    {
        if (fileSystem.Directory.EnumerateFiles(aspirateConfiguration.TemplatePath).Any())
        {
            return;
        }

        Logger.MarkupLine($"\r\nSelected Template directory [blue]'{aspirateConfiguration.TemplatePath}'[/] is empty.");
        var shouldCreate = Logger.Confirm("Would you like to populate it with the default templates?");

        if (!shouldCreate)
        {
            return;
        }

        foreach (var templateFile in fileSystem.Directory.EnumerateFiles(fileSystem.Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder)))
        {
            var templateFileName = fileSystem.Path.GetFileName(templateFile);
            fileSystem.File.Copy(templateFile, fileSystem.Path.Combine(aspirateConfiguration.TemplatePath, templateFileName));
            Logger.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] copied template [blue]'{templateFileName}'[/] to directory [blue]'{aspirateConfiguration.TemplatePath}'[/].");
        }
    }
}
