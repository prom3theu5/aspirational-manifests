namespace Aspirate.Cli.Commands.Build;

public sealed class BuildCommand : BaseCommand<BuildOptions, BuildCommandHandler>
{
    public BuildCommand() : base("build", "Builds and pushes containers")
    {
       AddOption(SharedOptions.AspireProjectPath);
       AddOption(SharedOptions.AspireManifest);
       AddOption(SharedOptions.ContainerBuilder);
       AddOption(SharedOptions.ContainerImageTag);
       AddOption(SharedOptions.ContainerRegistry);
    }
}
