using Aspirate.Shared.Interfaces.Secrets;

namespace Aspirate.Secrets.Providers.Password;

public class AesGcmCrypter : IEncrypter, IDecrypter
{
    private readonly byte[] _key;
    private readonly byte[] _saltBytes;
    private readonly int _tagSizeInBytes;

    public AesGcmCrypter(byte[] key, byte[] saltBytes, int tagSizeInBytes)
    {
        if (saltBytes.Length != 12)
        {
            throw new ArgumentException("The salt must be exactly 12 bytes.", nameof(saltBytes));
        }

        _key = key;
        _saltBytes = saltBytes;
        _tagSizeInBytes = tagSizeInBytes;
    }

    public string EncryptValue(string plaintext)
    {
        using var aesGcm = new AesGcm(_key, _tagSizeInBytes);

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertextBytes = new byte[plaintextBytes.Length];
        var tag = new byte[_tagSizeInBytes];

        aesGcm.Encrypt(_saltBytes, plaintextBytes, ciphertextBytes, tag);

        // Prepend the salt and the tag to the ciphertext
        var resultBytes = new byte[_saltBytes.Length + tag.Length + ciphertextBytes.Length];
        Buffer.BlockCopy(_saltBytes, 0, resultBytes, 0, _saltBytes.Length);
        Buffer.BlockCopy(tag, 0, resultBytes, _saltBytes.Length, tag.Length);
        Buffer.BlockCopy(ciphertextBytes, 0, resultBytes, _saltBytes.Length + tag.Length, ciphertextBytes.Length);

        return Convert.ToBase64String(resultBytes);
    }

    public string DecryptValue(string ciphertext)
    {
        using var aesGcm = new AesGcm(_key, _tagSizeInBytes);

        var ciphertextBytes = Convert.FromBase64String(ciphertext);

        // Extract the salt and the tag from the ciphertext
        var salt = new byte[_saltBytes.Length];
        var tag = new byte[_tagSizeInBytes];
        var actualCiphertextBytes = new byte[ciphertextBytes.Length - _saltBytes.Length - tag.Length];
        Buffer.BlockCopy(ciphertextBytes, 0, salt, 0, _saltBytes.Length);
        Buffer.BlockCopy(ciphertextBytes, _saltBytes.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(ciphertextBytes, _saltBytes.Length + tag.Length, actualCiphertextBytes, 0, actualCiphertextBytes.Length);

        var plaintextBytes = new byte[actualCiphertextBytes.Length];
        aesGcm.Decrypt(salt, actualCiphertextBytes, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }

    public Dictionary<string, string> BulkDecrypt(List<string> ciphertexts) =>
        ciphertexts.ToDictionary(ciphertext => ciphertext, DecryptValue);
}
