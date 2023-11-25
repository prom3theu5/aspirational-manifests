namespace Aspirate.Cli.Commands.Init;

public sealed class InitCommand : BaseCommand<InitOptions, InitCommandHandler>
{
    public InitCommand() : base("init", "Initializes aspirate settings within your AppHost directory.") =>
        AddOption(new Option<string>(new[] { "-p", "--project-path" })
        {
            Description = "The path to the aspire project",
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = false,
        });
}
