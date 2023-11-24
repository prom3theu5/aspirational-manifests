namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand(
    IAnsiConsole console,
    IAspirateConfigurationService aspirateConfigurationService,
    IFileSystem filesystem,
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

        AddTemplatesToTemplateDirectoryIfRequired(aspirateConfiguration);

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
        console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'Container fallback registry'[/] to [blue]'{aspirateConfiguration.ContainerSettings.Registry}'[/].");
        console.WriteLine();
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
        console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'Container fallback tag'[/] to [blue]'{aspirateConfiguration.ContainerSettings.Tag}'[/].");
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
        aspirateConfiguration.TemplatePath = filesystem.GetFullPath(templatePath);
        console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Set [blue]'TemplatePath'[/] to [blue]'{aspirateConfiguration.TemplatePath}'[/].");
        console.WriteLine();
    }

    private void AddTemplatesToTemplateDirectoryIfRequired(AspirateSettings aspirateConfiguration)
    {
        if (string.IsNullOrEmpty(aspirateConfiguration.TemplatePath))
        {
            return;
        }

        var templateDirectoryExists = filesystem.Directory.Exists(aspirateConfiguration.TemplatePath);

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
        console.MarkupLine($"\r\nSelected Template directory [blue]'{aspirateConfiguration.TemplatePath}'[/] does not exist.");
        var shouldCreate = console.Confirm("Would you like to create it?");

        if (!shouldCreate)
        {
            return false;
        }

        filesystem.Directory.CreateDirectory(aspirateConfiguration.TemplatePath);
        console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] Directory [blue]'{aspirateConfiguration.TemplatePath}'[/] has been created.");
        return true;
    }

    private void HandleCreateTemplateFiles(AspirateSettings aspirateConfiguration)
    {
        if (filesystem.Directory.EnumerateFiles(aspirateConfiguration.TemplatePath).Any())
        {
            return;
        }

        console.MarkupLine($"\r\nSelected Template directory [blue]'{aspirateConfiguration.TemplatePath}'[/] is empty.");
        var shouldCreate = console.Confirm("Would you like to populate it with the default templates?");

        if (!shouldCreate)
        {
            return;
        }

        foreach (var templateFile in filesystem.Directory.EnumerateFiles(filesystem.Path.Combine(AppContext.BaseDirectory, TemplateLiterals.TemplatesFolder)))
        {
            var templateFileName = filesystem.Path.GetFileName(templateFile);
            filesystem.File.Copy(templateFile, filesystem.Path.Combine(aspirateConfiguration.TemplatePath, templateFileName));
            console.MarkupLine($"\r\n[green]({EmojiLiterals.CheckMark}) Done:[/] copied template [blue]'{templateFileName}'[/] to directory [blue]'{aspirateConfiguration.TemplatePath}'[/].");
        }
    }
}
