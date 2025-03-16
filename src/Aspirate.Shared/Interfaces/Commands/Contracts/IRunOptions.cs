namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IRunOptions
{
    bool? AllowClearNamespace { get; set; }
    string? Namespace { get; set; }
    bool? SkipBuild { get; set; }
    string? ImagePullPolicy { get; set; }
    string? RuntimeIdentifier { get; set; }
}
