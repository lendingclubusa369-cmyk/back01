namespace lender_club.Services;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string MaskSsn(string ssn);
    string MaskAccountNumber(string accountNumber);
}
