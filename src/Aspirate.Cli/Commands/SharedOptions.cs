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

    public static Option<string> TemplatePath => new(new[] {"-tp", "--template-path" })
    {
        Description = "The Custom Template path to use.",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> ContainerRegistry => new(new[] {"-cr", "--container-registry" })
    {
        Description = "The Container Registry to use as the fall-back value for all containers.",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> ContainerImageTag => new(new[] {"-ct", "--container-image-tag" })
    {
        Description = "The Container Image Tag to use as the fall-back value for all containers.",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> ContainerBuilder => new(new[] { "--container-builder" })
    {
        Description = "The Container Builder: can be 'docker' or 'podman'. The default is 'docker'.",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> KubernetesContext => new(new[] { "-k", "--kube-context" })
    {
        Description = "The name of the kubernetes context to use",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<string> AspireManifest => new(new[] { "-m", "--aspire-manifest" })
    {
        Description = "The aspire manifest file to use",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = false,
    };

    public static Option<bool> SkipBuild => new(new[] { "--skip-build" })
    {
        Description = "Skips build and Push of containers",
        Arity = ArgumentArity.ZeroOrOne,
        IsRequired = false,
    };

    public static Option<bool> SkipFinalKustomizeGeneration => new(new[] { "-sf", "--skip-final", "--skip-final-kustomize-generation" })
    {
        Description = "Skips The final generation of the kustomize manifest, which is the parent top level file",
        Arity = ArgumentArity.ZeroOrOne,
        IsRequired = false,
    };
}
