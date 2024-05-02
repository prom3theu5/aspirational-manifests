namespace Aspirate.Shared.Interfaces.Secrets;

public interface IDecrypter
{
    string DecryptValue(string ciphertext);
    Dictionary<string, string> BulkDecrypt(List<string> ciphertexts);
}
