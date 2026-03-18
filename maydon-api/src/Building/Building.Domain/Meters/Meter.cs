using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.MeterReadings;
using Building.Domain.MeterTypes;
using Core.Domain.Entities;

namespace Building.Domain.Meters;
/// <summary>
/// Привязка к объекту
/// </summary>

[Table("meters", Schema = AssemblyReference.Instance)]
public sealed class Meter : Entity
{
	private Meter() { }
	public Meter(
		Guid realEstateId,
		Guid? realEstateUnitId,
		Guid meterTypeId,
		string serialNumber,
		DateOnly? installationDate,
		DateOnly? verificationDate,
		DateOnly? nextVerificationDate,
		decimal initialReading,
		bool isActive = true) : base()
	{
		RealEstateId = realEstateId;
		RealEstateUnitId = realEstateUnitId;
		MeterTypeId = meterTypeId;
		SerialNumber = serialNumber;
		InstallationDate = installationDate;
		VerificationDate = verificationDate;
		NextVerificationDate = nextVerificationDate;
		InitialReading = initialReading;
		IsActive = isActive;

	}
	public Guid RealEstateId { get; private set; }
	public Guid? RealEstateUnitId { get; private set; }
	public Guid MeterTypeId { get; private set; }
	[Required]
	[MaxLength(100)]
	public string SerialNumber { get; private set; }
	public DateOnly? InstallationDate { get; private set; }
	public DateOnly? VerificationDate { get; private set; }
	public DateOnly? NextVerificationDate { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal InitialReading { get; private set; }
	public bool IsActive { get; private set; }
	public MeterType MeterType { get; private set; }
	public ICollection<MeterReading> MeterReadings { get; private set; } = [];

	public Meter Update(
		Guid meterTypeId,
		string serialNumber,
		DateOnly? installationDate,
		DateOnly? verificationDate,
		DateOnly? nextVerificationDate,
		decimal initialReading)
	{
		MeterTypeId = meterTypeId;
		SerialNumber = serialNumber;
		InstallationDate = installationDate;
		VerificationDate = verificationDate;
		NextVerificationDate = nextVerificationDate;
		InitialReading = initialReading;
		return this;
	}
	public Meter Activate()
	{
		IsActive = true;
		return this;
	}
	public Meter Deactivate()
	{
		IsActive = false;
		return this;
	}
}
