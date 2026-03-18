namespace Identity.Application.Core.Abstractions.Cryptors;

public interface ICryptor
{
	string EncryptInvitation(Guid id);
	Guid DecryptInvitation(string key);

	string EncryptAccount(Guid accountId, Guid sessionId);
	(Guid AccountId, Guid SessionId)? DecryptAccount(string key);

	string EncryptUserState(Guid id);
	Guid? DecryptUserState(string key);
}
