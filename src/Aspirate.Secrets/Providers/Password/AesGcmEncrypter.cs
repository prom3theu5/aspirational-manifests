namespace Aspirate.Secrets.Providers.Password;

public class AesGcmEncrypter(byte[] key, int tagSizeInBytes) : IEncrypter
{
    public string EncryptValue(string plaintext)
    {
        using var aesGcm = new AesGcm(key, tagSizeInBytes);

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertextBytes = new byte[plaintextBytes.Length];
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[aesGcm.TagSizeInBytes.GetValueOrDefault()];

        // Generate a random nonce
        RandomNumberGenerator.Fill(nonce);

        aesGcm.Encrypt(nonce, plaintextBytes, ciphertextBytes, tag);

        // Prepend the nonce and the tag to the ciphertext
        var resultBytes = new byte[nonce.Length + tag.Length + ciphertextBytes.Length];
        Buffer.BlockCopy(nonce, 0, resultBytes, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, resultBytes, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertextBytes, 0, resultBytes, nonce.Length + tag.Length, ciphertextBytes.Length);

        return Convert.ToBase64String(resultBytes);
    }
}
