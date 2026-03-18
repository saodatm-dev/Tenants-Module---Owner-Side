using Core.Domain.Security;

namespace Core.Application.Abstractions.Security;

/// <summary>
/// Extended encryptor with binary output for optimized database storage.
/// Includes compression for large content.
/// </summary>
public interface IBinaryEncryptor : IStringEncryptor
{
	byte[] EncryptToBinary(string plaintext);
	string DecryptFromBinary(byte[] cipherData);
	EncryptionResult EncryptWithMetrics(string plaintext);
}
