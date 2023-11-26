namespace Aspirate.Cli.Commands.Build;

public sealed class BuildCommand : BaseCommand<BuildOptions, BuildCommandHandler>
{
    public BuildCommand() : base("build", "Builds and pushes containers")
    {
       AddOption(SharedOptions.AspireProjectPath);
       AddOption(SharedOptions.NonInteractive);
    }
}
