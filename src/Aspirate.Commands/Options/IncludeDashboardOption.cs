namespace Aspirate.Commands.Options;

public sealed class IncludeDashboardOption : BaseOption<bool?>
{
    private static readonly string[] _aliases = ["--include-dashboard", "--with-dashboard"];

    private IncludeDashboardOption() : base(_aliases, "ASPIRATE_INCLUDE_DASHBOARD", null)
    {
        Name = nameof(IDashboardOptions.IncludeDashboard);
        Description = "Include the Aspire Dashboard in the generated manifests";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static IncludeDashboardOption Instance { get; } = new();
}
