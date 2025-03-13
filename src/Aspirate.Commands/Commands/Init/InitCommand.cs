namespace Aspirate.Commands.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    protected override bool CommandUnlocksSecrets => false;
    protected override bool CommandSkipsStateAndSecrets => true;

    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.")
    {
        AddOption(ProjectPathOption.Instance);
        AddOption(ContainerBuilderOption.Instance);
        AddOption(ContainerBuildArgsOption.Instance);
        AddOption(ContainerBuildContextOption.Instance);
        AddOption(ContainerRegistryOption.Instance);
        AddOption(ContainerRepositoryPrefixOption.Instance);
        AddOption(ContainerImageTagOption.Instance);
        AddOption(TemplatePathOption.Instance);
    }
}
