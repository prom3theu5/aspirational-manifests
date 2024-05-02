namespace Aspirate.Shared.Interfaces.Secrets;

public interface IPasswordGenerator
{
    string Generate(int length = 24);
}
