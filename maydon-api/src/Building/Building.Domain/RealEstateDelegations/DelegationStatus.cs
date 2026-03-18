namespace Building.Domain.RealEstateDelegations;

public enum DelegationStatus
{
	Pending = 0,        // На рассмотрении
	Active,             // Активен
	Suspended,          // Приостановлен
	Expired,            // Истек
	Revoked             // Отозван
}
