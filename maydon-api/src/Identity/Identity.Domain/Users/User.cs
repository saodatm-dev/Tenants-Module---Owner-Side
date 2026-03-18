using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.Accounts;
using Identity.Domain.Accounts.Events;
using Identity.Domain.Users.Events;

namespace Identity.Domain.Users;

[Table("users", Schema = AssemblyReference.Instance)]
public sealed class User : Entity
{
	private User() { }
	public User(
		RegisterType registerType,
		string? firstName = null,
		string? lastName = null,
		string? middleName = null,
		string? phoneNumber = null,                                 // nullable if user has registered by oneid or pkcs 
		byte[]? password = null,
		byte[]? salt = null,
		string? tin = null,
		string? pinfl = null,
		string? serialNumber = null,
		string? passportNumber = null,
		DateOnly? birthDate = null) : base()
	{
		PhoneNumber = phoneNumber;
		FirstName = firstName;
		LastName = lastName;
		MiddleName = middleName;
		Password = password;
		Salt = salt;
		Tin = tin;
		Pinfl = pinfl;
		SerialNumber = serialNumber;
		PassportNumber = passportNumber;
		BirthDate = birthDate;

		Raise(new UpsertUserPostDomainEvent(this));
	}
	[MaxLength(100)]
	public string? FirstName { get; private set; }
	[MaxLength(100)]
	public string? LastName { get; private set; }
	[MaxLength(100)]
	public string? MiddleName { get; private set; }
	[EncryptColumn]
	[MaxLength(20)]
	public string? PhoneNumber { get; private set; }
	public byte[]? Password { get; private set; }
	public byte[]? Salt { get; private set; }
	[EncryptColumn]
	[MaxLength(15)]
	public string? Tin { get; private set; }
	[EncryptColumn]
	[MaxLength(14)]
	public string? Pinfl { get; private set; }
	[EncryptColumn]
	[MaxLength(100)]
	public string? SerialNumber { get; private set; }           // serial number of e-imzo
	[EncryptColumn]
	[MaxLength(20)]
	public string? PassportNumber { get; private set; }
	public DateOnly? BirthDate { get; private set; }
	public Guid? RegionId { get; private set; }
	public Guid? DistrictId { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	[MaxLength(500)]
	public string? ObjectName { get; private set; }
	public bool IsVerified { get; private set; }
	public RegisterType RegisterType { get; private set; }
	public bool IsActive { get; private set; } = true;
	public ICollection<Account> Accounts { get; private set; }

	[NotMapped]
	[JsonIgnore]
	public string FullName => $"{FirstName ?? string.Empty} {LastName ?? string.Empty} {MiddleName ?? string.Empty}".Trim();
	public User Update(User user)
	{
		DistrictId = user.DistrictId;
		ObjectName = user.ObjectName;
		FirstName = user.FirstName;
		LastName = user.LastName;
		MiddleName = user.MiddleName;
		Tin = user.Tin;
		Pinfl = user.Pinfl;

		RegionId = user.RegionId;
		DistrictId = user.DistrictId;
		Address = user.Address;

		Raise(new UpsertUserPostDomainEvent(this));
		return this;
	}
	public User UpdateIfEmpty(
		string? firstName = null,
		string? lastName = null,
		string? middleName = null,
		string? tin = null,
		string? pinfl = null,
		string? serialNumber = null,
		string? passportNumber = null,
		DateOnly? birthDate = null)
	{
		FirstName = FirstName ?? firstName;
		LastName = LastName ?? lastName;
		MiddleName = MiddleName ?? middleName;
		Tin = Tin ?? tin;
		Pinfl = Pinfl ?? pinfl;
		SerialNumber = SerialNumber ?? serialNumber;
		PassportNumber = PassportNumber ?? passportNumber;
		BirthDate = BirthDate ?? birthDate;
		Raise(new UpsertUserPostDomainEvent(this));
		return this;
	}

	public User UpdateLogo(string? objectName)
	{
		ObjectName = objectName;
		return this;
	}
	public User Remove()
	{
		Raise(new DeleteAccountPreDomainEvent(this.Id, this.Id));
		Raise(new DeleteUserPostDomainEvent(this.Id));
		return this;
	}
	public User ChangePassword(byte[] password, byte[] salt)
	{
		Password = password;
		Salt = salt;
		return this;
	}

	public User UpdateProfile(
		string? firstName = null,
		string? lastName = null,
		string? middleName = null,
		string? objectName = null)
	{
		FirstName = string.IsNullOrWhiteSpace(firstName) ? FirstName : firstName;
		LastName = string.IsNullOrWhiteSpace(lastName) ? LastName : lastName;
		MiddleName = string.IsNullOrWhiteSpace(middleName) ? MiddleName : middleName;
		ObjectName = string.IsNullOrWhiteSpace(objectName) ? ObjectName : objectName;

		Raise(new UpsertUserPostDomainEvent(this));
		return this;
	}
}
