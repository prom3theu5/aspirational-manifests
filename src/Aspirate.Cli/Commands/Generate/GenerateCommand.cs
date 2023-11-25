namespace Aspirate.Cli.Commands.Generate;

public sealed class GenerateCommand : BaseCommand<GenerateOptions, GenerateCommandHandler>
{
    public GenerateCommand() : base("generate", "Builds, pushes containers, generates aspire manifest and kustomize manifests.")
    {
        AddOption(new Option<string>(new[] { "-p", "--project-path"})
            {
                Description = "The path to the aspire project",
                Arity = ArgumentArity.ExactlyOne,
                IsRequired = false,
            });

        AddOption(new Option<string>(new[] { "-o", "--output-path"})
        {
            Description = "The output path for generated manifests",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
    }
}
