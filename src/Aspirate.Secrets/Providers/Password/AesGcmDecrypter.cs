namespace Aspirate.Secrets.Providers.Password;

public class AesGcmDecrypter(byte[] key, int tagSizeInBytes) : IDecrypter
{
    public string DecryptValue(string ciphertext)
    {
        using var aesGcm = new AesGcm(key, tagSizeInBytes);

        var ciphertextBytes = Convert.FromBase64String(ciphertext);

        // Extract the nonce and the tag from the ciphertext
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[aesGcm.TagSizeInBytes.GetValueOrDefault()];
        var actualCiphertextBytes = new byte[ciphertextBytes.Length - nonce.Length - tag.Length];
        Buffer.BlockCopy(ciphertextBytes, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(ciphertextBytes, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(ciphertextBytes, nonce.Length + tag.Length, actualCiphertextBytes, 0, actualCiphertextBytes.Length);

        var plaintextBytes = new byte[actualCiphertextBytes.Length];
        aesGcm.Decrypt(nonce, actualCiphertextBytes, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }

    public Dictionary<string, string> BulkDecrypt(List<string> ciphertexts) =>
        ciphertexts.ToDictionary(ciphertext => ciphertext, DecryptValue);
}
