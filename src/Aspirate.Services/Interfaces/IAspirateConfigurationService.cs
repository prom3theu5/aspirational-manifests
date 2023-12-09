namespace Aspirate.Services.Interfaces;

/// <summary>
/// Represents a service for handling Aspirate configuration.
/// </summary>
public interface IAspirateConfigurationService
{
    /// <summary>
    /// Handles an existing configuration.
    /// </summary>
    /// <param name="appHostPath">The path to the application host configuration file.</param>
    /// <param name="nonInteractive">Whether or not the handling process should be non-interactive (optional, default is false).</param>
    /// <remarks>
    /// This method is used to handle an existing application host configuration file. It takes the path to the
    /// configuration file and an optional flag to indicate whether the handling process should be non-interactive.
    /// If the nonInteractive flag is set to true, the handling process will not prompt the user for any input and
    /// will perform the necessary actions without user intervention.
    /// </remarks>
    /// <example>
    /// This method can be used as follows:
    /// <code>
    /// HandleExistingConfiguration("C:\AppHost.config", true);
    /// </code>
    /// </example>
    void HandleExistingConfiguration(string appHostPath, bool nonInteractive = false);

    /// <summary>
    /// Saves the configuration file for the specified AspirateSettings object.
    /// </summary>
    /// <param name="settings">The AspirateSettings object containing the configuration settings.</param>
    /// <param name="appHostPath">The path to the application host where the configuration file will be saved.</param>
    /// <remarks>
    /// This method saves the configuration file for the given AspirateSettings object. The configuration file contains the
    /// necessary settings for the application to function properly. The file will be saved at the specified application
    /// host path.
    /// </remarks>
    void SaveConfigurationFile(AspirateSettings settings, string appHostPath);

    /// <summary>
    /// Loads the configuration file from the specified app host path.
    /// </summary>
    /// <param name="appHostPath">The path to the app host.</param>
    /// <returns>The loaded AspirateSettings object, or null if the configuration file could not be found or loaded.</returns>
    AspirateSettings? LoadConfigurationFile(string appHostPath);
}
