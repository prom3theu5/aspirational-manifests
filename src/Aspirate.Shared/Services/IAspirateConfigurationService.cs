namespace Aspirate.Shared.Services;

public interface IAspirateConfigurationService
{
    void HandleExistingConfiguration(string appHostPath, bool nonInteractive = false);
    void SaveConfigurationFile(AspirateSettings settings, string appHostPath);
    AspirateSettings? LoadConfigurationFile(string appHostPath);
}
