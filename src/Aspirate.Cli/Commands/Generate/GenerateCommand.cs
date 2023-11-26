namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateCommand : BaseCommand<GenerateOptions, GenerateCommandHandler>
{
    public GenerateCommand() : base("generate", "Builds, pushes containers, generates aspire manifest and kustomize manifests.")
    {
       AddOption(SharedOptions.AspireProjectPath);
       AddOption(SharedOptions.AspireManifest);
       AddOption(SharedOptions.OutputPath);
       AddOption(SharedOptions.NonInteractive);
       AddOption(SharedOptions.SkipBuild);
    }
}
