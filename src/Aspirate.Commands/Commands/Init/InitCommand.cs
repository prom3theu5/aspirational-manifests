namespace Aspirate.Commands.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.")
    {
        AddOption(SharedOptions.AspireProjectPath);
        AddOption(SharedOptions.ContainerRegistry);
        AddOption(SharedOptions.ContainerImageTag);
        AddOption(SharedOptions.TemplatePath);
    }
}
