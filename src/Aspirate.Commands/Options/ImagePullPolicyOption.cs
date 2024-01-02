namespace Aspirate.Commands.Options;

public sealed class ImagePullPolicyOption : BaseOption<string?>
{
    private static readonly string[] _aliases = { "--image-pull-policy" };

    private ImagePullPolicyOption() : base(_aliases, "ASPIRATE_IMAGE_PULL_POLICY", null)
    {
        Name = nameof(IGenerateOptions.ImagePullPolicy);
        Description = "The Image pull policy to use when generating manifests";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
        this.AddValidator(ValidateFormat);
    }

    public static ImagePullPolicyOption Instance { get; } = new();

    private static void ValidateFormat(OptionResult optionResult)
    {
        var value = optionResult.GetValueOrDefault<string?>();

        if (value is null)
        {
            return;
        }

        if (!ImagePullPolicy.TryFromValue(value, out _))
        {
            throw new ArgumentException($"--image-pull-policy must be either '{ImagePullPolicy.IfNotPresent.Value}', '{ImagePullPolicy.Always.Value}' or '{ImagePullPolicy.Never.Value}'. It is case sensitive, and must not be quoted.");
        }
    }
}
