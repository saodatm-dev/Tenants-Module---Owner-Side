using System.Text.Json.Serialization;

namespace Identity.Application.Users.Profile;

public sealed record ProfileResponse(
	Guid UserId,
	bool IsIndividual,
	string PhoneNumber,
	string FirstName,
	string LastName,
	string? MiddleName,
	string? Photo,
	bool IsVerified,
	IEnumerable<string> Permissions,
	[property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	string? CompanyName=null,
	[property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	string? CompanyTin=null);
