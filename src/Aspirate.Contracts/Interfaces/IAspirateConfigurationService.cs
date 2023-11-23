namespace Aspirate.Contracts.Interfaces;

public interface IAspirateConfigurationService
{
    void HandleExistingConfiguration(string appHostPath);
    void SaveConfigurationFile(AspirateSettings settings, string appHostPath);
    AspirateSettings? LoadConfigurationFile(string appHostPath);
}
