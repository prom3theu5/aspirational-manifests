namespace Aspirate.Cli.Commands;

public static class SharedOptions
{
    public static Option<string> AspireProjectPath => new(new[] {"-p", "--project-path" })
    {
        Description = "The path to the aspire project",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> OutputPath => new(new[] { "-o", "--output-path" })
    {
        Description = "The output path for generated manifests",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> ManifestDirectoryPath => new(new[] { "-i", "--input-path" })
    {
        Description = "The path for the kustomize manifests directory",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<bool> NonInteractive => new(new[] { "--non-interactive" })
    {
        Description = "Disables interactive mode for the command",
        Arity = ArgumentArity.ZeroOrOne,
        IsRequired = false,
    };
}
