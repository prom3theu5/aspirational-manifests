namespace Aspirate.Secrets.Providers.Base64;

public class Base64Encrypter : IEncrypter
{
    public string EncryptValue(string plaintext)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        return Convert.ToBase64String(plaintextBytes);
    }
}
