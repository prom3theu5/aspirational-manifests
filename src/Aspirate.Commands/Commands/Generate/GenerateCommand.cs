namespace Aspirate.Commands.Commands.Generate;

public sealed class GenerateCommand : BaseCommand<GenerateOptions, GenerateCommandHandler>
{
    public GenerateCommand() : base("generate", "Builds, pushes containers, generates aspire manifest and kustomize manifests.")
    {
       AddOption(SharedOptions.AspireProjectPath);
       AddOption(SharedOptions.AspireManifest);
       AddOption(SharedOptions.OutputPath);
       AddOption(SharedOptions.SkipBuild);
       AddOption(SharedOptions.SkipFinalKustomizeGeneration);
       AddOption(SharedOptions.ContainerBuilder);
       AddOption(SharedOptions.ContainerImageTag);
       AddOption(SharedOptions.ContainerRegistry);
    }
}