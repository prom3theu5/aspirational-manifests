namespace Aspirate.Commands.Options;

public sealed class ImagePullPolicyOption : BaseOption<string?>
{
    private static readonly string[] _aliases = { "--image-pull-policy" };

    private ImagePullPolicyOption() : base(_aliases, "ASPIRATE_IMAGE_PULL_POLICY", null)
    {
        Name = nameof(GenerateOptions.ImagePullPolicy);
        Description = "The Image pull policy to use when generating manifests";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static ImagePullPolicyOption Instance { get; } = new();
}
