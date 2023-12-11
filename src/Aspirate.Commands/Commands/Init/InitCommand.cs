namespace Aspirate.Commands.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.")
    {
        AddOption(ProjectPathOption.Instance);
        AddOption(ContainerRegistryOption.Instance);
        AddOption(ContainerImageTagOption.Instance);
        AddOption(TemplatePathOption.Instance);
    }
}
