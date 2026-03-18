using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.Companies.Events;
using Identity.Domain.Users;

namespace Identity.Domain.Companies;

[Table("companies", Schema = AssemblyReference.Instance)]
public sealed class Company : Entity
{
	private Company() { }
	public Company(
		Guid ownerId,
		string name,
		RegisterType registerType,
		string? tin = null,
		string? serialNumber = null,
		bool isVerified = false) : base()
	{
		OwnerId = ownerId;
		Name = name;
		RegisterType = registerType;
		Tin = tin;
		SerialNumber = serialNumber;
		IsVerified = isVerified;

		Raise(new CreateOrUpdateCompanyPostDomainEvent(this));
	}

	public Guid OwnerId { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Name { get; private set; }
	[MaxLength(15)]
	public string? Tin { get; private set; }
	[EncryptColumn]
	[MaxLength(100)]
	public string? SerialNumber { get; private set; }
	public bool IsVerified { get; private set; }
	public Guid? RegionId { get; private set; }
	public Guid? DistrictId { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	public RegisterType RegisterType { get; private set; }
	[MaxLength(500)]
	public string? ObjectName { get; private set; }
	public bool IsActive { get; private set; } = true;
	public Company UpdateOwner(Guid ownerId)
	{
		this.OwnerId = ownerId;
		return this;
	}
	public Company Update(
		string name,
		string? tin,
		Guid? regionId,
		Guid? districtId,
		string? address)
	{
		Name = name;
		Tin = tin;
		RegionId = regionId;
		DistrictId = districtId;
		Address = address;
		Raise(new CreateOrUpdateCompanyPostDomainEvent(this));
		return this;
	}
	public Company UpdateIfEmpty(
		string name,
		string? tin = null,
		string? serialNumber = null,
		bool isVerified = false)
	{
		Name = name;
		Tin = tin;
		SerialNumber = serialNumber;
		IsVerified = isVerified;
		Raise(new CreateOrUpdateCompanyPostDomainEvent(this));
		return this;
	}
	public Company UpdateLogo(string? objectName)
	{
		ObjectName = objectName;
		return this;
	}
	public Company Remove()
	{
		Raise(new DeleteCompanyPostDomainEvent(this.Id));
		return this;
	}
	public Company Activate()
	{
		IsActive = true;
		Raise(new CreateOrUpdateCompanyPostDomainEvent(this));
		return this;
	}
	public Company Deactivate()
	{
		IsActive = false;
		Raise(new CreateOrUpdateCompanyPostDomainEvent(this));
		return this;
	}
}
