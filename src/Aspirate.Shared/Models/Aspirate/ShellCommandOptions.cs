namespace Aspirate.Shared.Models.Aspirate;

[ExcludeFromCodeCoverage]
public sealed class ShellCommandOptions
{
    public string? Command { get; set; }
    public ArgumentsBuilder? ArgumentsBuilder { get; set; }
    public bool NonInteractive { get; set; }
    public Func<string, ArgumentsBuilder, bool, string, Task>? OnFailed { get; set; }
    public bool ShowOutput { get; set; } = false;
    public string? WorkingDirectory { get; set; }
    public char PropertyKeySeparator { get; set; } = ' ';
    public string? PreCommandMessage { get; set; }
    public string? SuccessCommandMessage { get; set; }
    public string? FailureCommandMessage { get; set; }

    public bool ExitWithExitCode { get; set; } = false;
}
