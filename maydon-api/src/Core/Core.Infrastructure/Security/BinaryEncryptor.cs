using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Core.Application.Abstractions.Security;
using Core.Domain.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Security;

public class BinaryEncryptor(
	IOptionsMonitor<EncryptionOptions> optionsMonitor,
	ILogger<BinaryEncryptor>? logger = null)
	: IBinaryEncryptor
{
	private const int NonceSize = 12;   // 96 bits for AES-GCM
	private const int TagSize = 16;     // 128 bits auth tag
	private const int KeySize = 32;     // 256 bits
	private const byte VersionWithCompression = 0x01;
	private const byte VersionWithoutCompression = 0x00;

	private readonly IOptionsMonitor<EncryptionOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));

	#region IStringEncryptor (Backward Compatible - Base64 output)

	public string Encrypt(string plaintext)
	{
		if (string.IsNullOrEmpty(plaintext))
			return plaintext;

		var encrypted = EncryptToBinary(plaintext);
		return Convert.ToBase64String(encrypted);
	}

	public string Decrypt(string ciphertext)
	{
		if (string.IsNullOrEmpty(ciphertext))
			return ciphertext;

		var cipherData = Convert.FromBase64String(ciphertext);
		return DecryptFromBinary(cipherData);
	}

	#endregion

	#region IBinaryEncryptor (Optimized - Binary output)

	public byte[] EncryptToBinary(string plaintext)
	{
		if (string.IsNullOrEmpty(plaintext))
			return [];

		var options = _optionsMonitor.CurrentValue;
		var plainBytes = Encoding.UTF8.GetBytes(plaintext);

		byte[] dataToEncrypt;
		byte version;

		if (options.EnableCompression)
		{
			dataToEncrypt = Compress(plainBytes, options.CompressionLevel);
			version = VersionWithCompression;
		}
		else
		{
			dataToEncrypt = plainBytes;
			version = VersionWithoutCompression;
		}

		var key = DeriveKey(options.PassKey);
		var nonce = RandomNumberGenerator.GetBytes(NonceSize);
		var cipherText = new byte[dataToEncrypt.Length];
		var tag = new byte[TagSize];

		using var aes = new AesGcm(key, TagSize);
		aes.Encrypt(nonce, dataToEncrypt, cipherText, tag);

		// Format: [version:1][nonce:12][tag:16][ciphertext:N]
		var result = new byte[1 + NonceSize + TagSize + cipherText.Length];
		result[0] = version;
		Buffer.BlockCopy(nonce, 0, result, 1, NonceSize);
		Buffer.BlockCopy(tag, 0, result, 1 + NonceSize, TagSize);
		Buffer.BlockCopy(cipherText, 0, result, 1 + NonceSize + TagSize, cipherText.Length);

		return result;
	}

	public string DecryptFromBinary(byte[] cipherData)
	{
		if (cipherData is null || cipherData.Length < 1 + NonceSize + TagSize)
			return string.Empty;

		var version = cipherData[0];
		var nonce = new byte[NonceSize];
		var tag = new byte[TagSize];
		var cipherText = new byte[cipherData.Length - 1 - NonceSize - TagSize];

		Buffer.BlockCopy(cipherData, 1, nonce, 0, NonceSize);
		Buffer.BlockCopy(cipherData, 1 + NonceSize, tag, 0, TagSize);
		Buffer.BlockCopy(cipherData, 1 + NonceSize + TagSize, cipherText, 0, cipherText.Length);

		var key = DeriveKey(_optionsMonitor.CurrentValue.PassKey);
		var decrypted = new byte[cipherText.Length];

		using var aes = new AesGcm(key, TagSize);
		aes.Decrypt(nonce, cipherText, tag, decrypted);

		var plainBytes = version == VersionWithCompression
			? Decompress(decrypted)
			: decrypted;

		return Encoding.UTF8.GetString(plainBytes);
	}

	public EncryptionResult EncryptWithMetrics(string plaintext)
	{
		if (string.IsNullOrEmpty(plaintext))
			return new EncryptionResult([], 0, 0, 0);

		var originalBytes = Encoding.UTF8.GetBytes(plaintext);
		var options = _optionsMonitor.CurrentValue;

		var compressed = options.EnableCompression
			? Compress(originalBytes, options.CompressionLevel)
			: originalBytes;

		var encrypted = EncryptToBinary(plaintext);

		var result = new EncryptionResult(
			encrypted,
			originalBytes.Length,
			compressed.Length,
			encrypted.Length);

		logger?.LogDebug("Encryption: {Summary}", result.Summary);

		return result;
	}

	#endregion

	#region Private Methods

	private static byte[] DeriveKey(string passKey)
	{
		var salt = "VDn3DPkRenudsZAMMZk4XpZj5T4C4ap6"u8.ToArray();

		return Rfc2898DeriveBytes.Pbkdf2(
			passKey,
			salt,
			iterations: 100_000,
			HashAlgorithmName.SHA256,
			KeySize);
	}

	private static byte[] Compress(byte[] data, CompressionLevel level)
	{
		using var output = new MemoryStream();
		using (var gzip = new GZipStream(output, level, leaveOpen: true))
		{
			gzip.Write(data);
		}
		return output.ToArray();
	}

	private static byte[] Decompress(byte[] compressed)
	{
		using var input = new MemoryStream(compressed);
		using var gzip = new GZipStream(input, CompressionMode.Decompress);
		using var output = new MemoryStream();
		gzip.CopyTo(output);
		return output.ToArray();
	}

	#endregion
}
