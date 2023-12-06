namespace Aspirate.Secrets.Services;

public interface IEncrypter
{
    string EncryptValue(string plaintext);
}
