namespace Identity.Application.Core.Abstractions.Authentication;

public interface IPasswordHasher
{
	bool TryHash(string password, out byte[] passwordHash, out byte[] salt);
	bool Verify(string password, byte[] passwordHash, byte[] salt);
}
