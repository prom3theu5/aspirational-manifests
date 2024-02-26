namespace Aspirate.Commands.Options;

public sealed class RollingRestartOption : BaseOption<bool>
{
    private static readonly string[] _aliases =
    {
        "-r",
        "--rolling-restart",
    };

    private RollingRestartOption() : base(_aliases, "ASPIRATE_ROLLING_RESTART", true)
    {
        Name = nameof(IApplyOptions.RollingRestart);
        Description = "Indicates if a rolling restart should occur at the end of deploy. Defaults to 'true'.";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static RollingRestartOption Instance { get; } = new();
}
