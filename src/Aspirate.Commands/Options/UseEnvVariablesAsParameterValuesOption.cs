namespace Aspirate.Commands.Options;

public sealed class UseEnvVariablesAsParameterValuesOption : BaseOption<bool>
{
    private static readonly string[] _aliases = ["--use-env-variables-as-parameter-values"];

    private UseEnvVariablesAsParameterValuesOption() : base(_aliases, "ASPIRATE_USE_ENV_VARIABLES", false)
    {
        Name = nameof(IGenerateOptions.UseEnvVariablesAsParameterValues);
        Description = "Replace parameter references with enviroment variable names";
        Arity = ArgumentArity.ZeroOrOne;
        IsRequired = false;
    }

    public static UseEnvVariablesAsParameterValuesOption Instance { get; } = new();
}
