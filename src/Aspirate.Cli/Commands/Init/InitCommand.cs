namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.")
    {
        AddOption(SharedOptions.AspireProjectPath);

        AddOption(new Option<string>(new[] {"-cr", "--container-registry" })
        {
            Description = "The Container Registry to use as the fall-back value for all containers.",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });

        AddOption(new Option<string>(new[] {"-ct", "--container-image-tag" })
        {
            Description = "The Container Image Tag to use as the fall-back value for all containers.",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });

        AddOption(new Option<string>(new[] {"-tp", "--template-path" })
        {
            Description = "The Custom Template path to use.",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });

        AddOption(SharedOptions.NonInteractive);
    }
}
