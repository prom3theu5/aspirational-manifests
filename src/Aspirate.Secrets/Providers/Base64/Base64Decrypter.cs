namespace Aspirate.Secrets.Providers.Base64;

public class Base64Decrypter : IDecrypter
{
    public string DecryptValue(string ciphertext)
    {
        var ciphertextBytes = Convert.FromBase64String(ciphertext);
        return Encoding.UTF8.GetString(ciphertextBytes);
    }

    public Dictionary<string, string> BulkDecrypt(List<string> ciphertexts) =>
        ciphertexts.ToDictionary(ciphertext => ciphertext, DecryptValue);
}
