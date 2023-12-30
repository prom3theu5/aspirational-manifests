namespace Aspirate.Commands.Contracts;

public interface IGenerateOptions
{
    string? OutputPath { get; set; }
    string? Namespace { get; set; }
    bool SkipBuild { get; set; }
    bool SkipFinalKustomizeGeneration { get; set; }
    string? ImagePullPolicy { get; set; }
    string? OutputFormat { get; set; }
}
