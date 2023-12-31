namespace Aspirate.Commands.Contracts;

public interface IPrivateRegistryCredentialsOptions
{
    string? RegistryUsername { get; set; }

    string? RegistryPassword { get; set; }

    string? RegistryEmail { get; set; }
    bool?  WithPrivateRegistry { get; set; }
}
