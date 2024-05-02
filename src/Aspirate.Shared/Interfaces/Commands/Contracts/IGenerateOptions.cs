namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IGenerateOptions
{
    string? OutputPath { get; set; }
    string? Namespace { get; set; }
    bool SkipBuild { get; set; }
    bool SkipFinalKustomizeGeneration { get; set; }
    bool SkipHelmGeneration { get; set; }
    string? ImagePullPolicy { get; set; }
    string? OutputFormat { get; set; }
}
