namespace Aspirate.Commands.Options;

public sealed class VerifyImageAgeOption : BaseOption<bool?>
{
    private static readonly string[] _aliases =
    [
        "--verify-image-age"
    ];

    private VerifyImageAgeOption() : base(_aliases, "ASPIRATE_VERIFY_IMAGE_AGE", null)
    {
        Name = nameof(IBuildOptions.VerifyImageAge);
        Description = "Verifies the age of published images";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static VerifyImageAgeOption Instance { get; } = new();
}
