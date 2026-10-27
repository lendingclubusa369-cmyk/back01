using System.Security.Cryptography;
using System.Text;

namespace lender_club.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(IConfiguration configuration)
    {
        var keyHex = configuration["Security:EncryptionKey"] 
                     ?? "4f8a9e2b7c6d1e3f5a8b0c2d4e6f8a9b4f8a9e2b7c6d1e3f5a8b0c2d4e6f8a9b";

        if (keyHex.Length == 64)
        {
            _key = Convert.FromHexString(keyHex);
        }
        else
        {
            using var sha = SHA256.Create();
            _key = sha.ComputeHash(Encoding.UTF8.GetBytes(keyHex));
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        
        // Write the 16-byte IV to the beginning of the stream
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);

        return sr.ReadToEnd();
    }

    public string MaskSsn(string ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
            return string.Empty;

        var digits = new string(ssn.Where(char.IsDigit).ToArray());
        if (digits.Length >= 4)
        {
            var last4 = digits[^4..];
            return $"***-**-{last4}";
        }
        return "***-**-****";
    }

    public string MaskAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return string.Empty;

        var trimmed = accountNumber.Trim();
        if (trimmed.Length > 4)
        {
            var last4 = trimmed[^4..];
            return new string('*', trimmed.Length - 4) + last4;
        }
        return new string('*', trimmed.Length);
    }
}
