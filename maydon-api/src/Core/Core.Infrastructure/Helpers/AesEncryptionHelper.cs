using System.Security.Cryptography;
using System.Text;

namespace Core.Infrastructure.Helpers;

public static class AesEncryptionHelper
{
	public static string Encrypt(string plainText, string key)
	{
		try
		{
			using Aes aes = Aes.Create();
			aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key)); // Generate a 256-bit key
			aes.GenerateIV(); // Create a random IV for each encryption

			using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
			using var ms = new MemoryStream();
			ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV to ciphertext

			using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
			using (var writer = new StreamWriter(cs))
			{
				writer.Write(plainText);
			}

			return Convert.ToBase64String(ms.ToArray());
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	// Decrypt a cipher text string using AES-256
	public static string Decrypt(string cipherText, string key)
	{
		try
		{
			byte[] fullCipher = Convert.FromBase64String(cipherText);
			using Aes aes = Aes.Create();
			aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));

			// Extract IV from the beginning
			byte[] iv = new byte[aes.BlockSize / 8];
			Array.Copy(fullCipher, iv, iv.Length);

			int cipherStartIndex = iv.Length;
			int cipherLength = fullCipher.Length - cipherStartIndex;

			using var decryptor = aes.CreateDecryptor(aes.Key, iv);
			using var ms = new MemoryStream(fullCipher, cipherStartIndex, cipherLength);
			using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
			using var reader = new StreamReader(cs);
			return reader.ReadToEnd();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}
}
