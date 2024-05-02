namespace Aspirate.Shared.Interfaces.Secrets;

public interface IEncrypter
{
    string EncryptValue(string plaintext);
}
