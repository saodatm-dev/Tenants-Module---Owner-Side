using System.Security.Cryptography;
using System.Text;
using Identity.Application.Core.Abstractions.Authentication;

namespace Identity.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
	public bool TryHash(string password, out byte[] passwordHash, out byte[] salt)
	{
		if (string.IsNullOrWhiteSpace(password))
		{
			passwordHash = Array.Empty<byte>();
			salt = Array.Empty<byte>();
			return false;
		}

		using var hmac = new HMACSHA512();
		salt = hmac.Key;
		passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		return true;
	}

	public bool Verify(string password, byte[] passwordHash, byte[] salt)
	{
		if (string.IsNullOrWhiteSpace(password) || passwordHash.Length != 64 || salt.Length != 128)
			return false;

		using var hmac = new HMACSHA512(salt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		return computedHash.SequenceEqual(passwordHash);
	}
}
