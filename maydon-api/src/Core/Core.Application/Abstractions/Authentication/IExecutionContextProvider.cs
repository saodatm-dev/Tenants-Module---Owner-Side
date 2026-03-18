namespace Core.Application.Abstractions.Authentication;

public interface IExecutionContextProvider
{
	const string
		AccountIdKey = "account_id",
		SessionIdKey = "session_id",
		TenantIdKey = "tenant_id",
		UserIdKey = "user_id",
		RoleIdKey = "role_id",
		IsOwnerKey = "is_owner",
		AccountTypeKey = "account_type",
		IsIndividualKey = "is_individual";
	//UserNameKey = "user_name",
	//CompanyNameKey = "company_name";

	Guid AccountId { get; }
	Guid SessionId { get; }
	Guid TenantId { get; }
	Guid UserId { get; }
	Guid RoleId { get; }
	bool IsIndividual { get; }
	bool IsOwner { get; }
	int? AccountType { get; }
	bool IsAuthorized { get; }
	string LanguageShortCode { get; }
	string IpAddress { get; }
	string UserAgent { get; }
}
