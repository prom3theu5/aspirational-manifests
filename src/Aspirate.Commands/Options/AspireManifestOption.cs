namespace Aspirate.Commands.Options;

public sealed class AspireManifestOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    [
        "-m",
        "--aspire-manifest"
    ];

    private AspireManifestOption() : base(_aliases,  "ASPIRATE_ASPIRE_MANIFEST_PATH", null)
    {
        Name = nameof(IAspireOptions.AspireManifest);
        Description ="The aspire manifest file to use";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static AspireManifestOption Instance { get; } = new();

    public override bool IsSecret => false;
}
