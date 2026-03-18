using Core.Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Infrastructure.Database;

/// <summary>
/// For VARBINARY storage (optimized - use this for large content).
/// </summary>
public class EncryptedBinaryConverter : ValueConverter<string, byte[]>
{
	public EncryptedBinaryConverter(IBinaryEncryptor encryptor)
		: base(
			plainText => encryptor.EncryptToBinary(plainText),
			cipherData => encryptor.DecryptFromBinary(cipherData))
	{
	}
}

public class NullableEncryptedBinaryConverter : ValueConverter<string?, byte[]?>
{
	public NullableEncryptedBinaryConverter(IBinaryEncryptor encryptor)
		: base(
			plainText => plainText == null ? null : encryptor.EncryptToBinary(plainText),
			cipherData => cipherData == null ? null : encryptor.DecryptFromBinary(cipherData))
	{
	}
}

/// <summary>
/// For NVARCHAR storage (legacy - use for small strings or backward compat).
/// </summary>
public class EncryptedStringConverter : ValueConverter<string, string>
{
	public EncryptedStringConverter(IStringEncryptor encryptor)
		: base(
			plainText => encryptor.Encrypt(plainText),
			cipherText => encryptor.Decrypt(cipherText))
	{
	}
}
