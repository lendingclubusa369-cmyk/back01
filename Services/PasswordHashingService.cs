using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace lender_club.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 128 / 8; // 16 bytes
    private const int KeySize = 256 / 8;  // 32 bytes
    private const int Iterations = 100000;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] subkey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize);

        var outputBytes = new byte[1 + 4 + 4 + SaltSize + KeySize];
        outputBytes[0] = 0x01; // Format marker
        
        // Write prf (HMACSHA256 = 1)
        WriteNetworkByteOrder(outputBytes, 1, 1);
        // Write iteration count
        WriteNetworkByteOrder(outputBytes, 5, Iterations);
        // Write salt
        Buffer.BlockCopy(salt, 0, outputBytes, 9, SaltSize);
        // Write subkey
        Buffer.BlockCopy(subkey, 0, outputBytes, 9 + SaltSize, KeySize);

        return Convert.ToBase64String(outputBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        byte[] decodedHashedPassword;
        try
        {
            decodedHashedPassword = Convert.FromBase64String(hashedPassword);
        }
        catch
        {
            return false;
        }

        if (decodedHashedPassword.Length < 1 + 4 + 4 + SaltSize + KeySize || decodedHashedPassword[0] != 0x01)
            return false;

        int iterations = ReadNetworkByteOrder(decodedHashedPassword, 5);

        var salt = new byte[SaltSize];
        Buffer.BlockCopy(decodedHashedPassword, 9, salt, 0, salt.Length);

        var expectedSubkey = new byte[KeySize];
        Buffer.BlockCopy(decodedHashedPassword, 9 + SaltSize, expectedSubkey, 0, expectedSubkey.Length);

        var actualSubkey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: KeySize);

        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, int value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }

    private static int ReadNetworkByteOrder(byte[] buffer, int offset)
    {
        return ((int)buffer[offset + 0] << 24)
            | ((int)buffer[offset + 1] << 16)
            | ((int)buffer[offset + 2] << 8)
            | ((int)buffer[offset + 3]);
    }
}
