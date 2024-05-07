namespace Aspirate.Shared.Interfaces.Services;

public interface IVersionCheckService
{
    Task CheckVersion();
    Task SetUpdateChecks(bool isEnabled);
}
