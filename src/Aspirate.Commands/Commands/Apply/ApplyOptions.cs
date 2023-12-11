namespace Aspirate.Commands.Commands.Apply;

public sealed class ApplyOptions : BaseCommandOptions, IApplyOptions
{
    public string? InputPath { get; set; }
    public string? KubeContext { get; set; }

    public string? SecretPassword { get; set; }
}
