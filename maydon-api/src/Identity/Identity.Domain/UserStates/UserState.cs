using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Identity.Domain.UserStates;

[Table("user_state", Schema = AssemblyReference.Instance)]
public sealed class UserState : Entity
{
	private UserState() { }
	public UserState(
		string phoneNumber,
		DateTime expiredTime,
		bool isRegistration = true) : base()
	{
		PhoneNumber = phoneNumber;
		ExpiredTime = expiredTime;
		IsRegistration = isRegistration;
	}
	[EncryptColumn]
	[MaxLength(20)]
	public string? PhoneNumber { get; private set; }
	public bool IsRegistration { get; private set; } = true;
	public DateTime ExpiredTime { get; private set; }
	public bool IsActive { get; private set; } = true;
	public bool IsStateActive(DateTime utcNow) => this.IsActive && ExpiredTime > utcNow;
	public UserState Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
