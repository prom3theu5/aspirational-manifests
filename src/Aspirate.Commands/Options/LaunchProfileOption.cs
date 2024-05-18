namespace Aspirate.Commands.Options;

public sealed class LaunchProfileOption : BaseOption<string?>
{
    private static readonly string[] _aliases =
    [
        "-lp",
        "--launch-profile"
    ];

    private LaunchProfileOption() : base(_aliases, "ASPIRATE_LAUNCH_PROFILE", null)
    {
        Name = nameof(ICommandOptions.LaunchProfile);
        Description = "The launch profile to use when building the aspire manifest from the AppHost.";
        Arity = ArgumentArity.ExactlyOne;
        IsRequired = false;
    }

    public static LaunchProfileOption Instance { get; } = new();
}
