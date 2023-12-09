namespace Aspirate.Services.Interfaces;

public interface IPasswordGenerator
{
    string Generate(int length = 24);
}
