namespace Core.Application.Abstractions.Security;

/// <summary>
/// Service for encrypting and decrypting sensitive strings.
/// </summary>
public interface IStringEncryptor
{
	string Encrypt(string plaintext);
	string Decrypt(string ciphertext);
}
