namespace Aspirate.Commands.Options;

public sealed class ProjectPathOption : BaseOption<string>
{
    private static readonly string[] _aliases =
    [
        "-p",
        "--project-path"
    ];

    private ProjectPathOption() : base(_aliases, "ASPIRATE_PROJECT_PATH", AspirateLiterals.DefaultAspireProjectPath)
    {
        Name = nameof(IAspireOptions.ProjectPath);
        Description =  "The path to the aspire project";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ProjectPathOption Instance { get; } = new();
}
